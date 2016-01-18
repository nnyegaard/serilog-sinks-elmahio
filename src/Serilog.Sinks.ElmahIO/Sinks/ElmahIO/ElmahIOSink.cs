namespace Serilog.Sinks.ElmahIO.Sinks.ElmahIO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using Core;
    using Events;
    using Newtonsoft.Json;

    public class ElmahIOSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;
        private readonly Guid _logId;

        public ElmahIOSink(IFormatProvider formatProvider, Guid logId)
        {
            _formatProvider = formatProvider;
            _logId = logId;
        }

        /// <summary>
        /// Emit the provided log event to the sink.
        /// </summary>
        /// <param name="logEvent">The log event to write.</param>
        public void Emit(LogEvent logEvent)
        {
            LogEventPropertyValue eventId;
            logEvent.Properties.TryGetValue("EventId", out eventId);

            LogEventPropertyValue hostName;
            logEvent.Properties.TryGetValue("Host", out hostName);

            LogEventPropertyValue url;
            logEvent.Properties.TryGetValue("RequestPath", out url);

            Message message = new Message(logEvent.RenderMessage(_formatProvider))
            {
                id = eventId?.ToString(),
                hostname = hostName?.ToString(),
                url = url?.ToString(),
                QueryString = RequestQueryToQuery(logEvent),
                Severity = LevelToSeverity(logEvent),
                DateTime = logEvent.Timestamp.DateTime.ToUniversalTime(),
                Detail = logEvent.Exception?.ToString(),
                Data = PropertiesToData(logEvent),
            };

            // Convert to json
            string messageJson = MessageToJson(message);

            // Call ElmhaIO     
            HttpClient client = new HttpClient();
            client.PostAsync($@"https://elmah.io/api/v2/messages?logid={_logId}", new StringContent(messageJson, Encoding.UTF8, "application/json"));
        }

        /// <summary>
        /// Convert Serilog LogEvent query parameters to ElmahIO querystring 
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> RequestQueryToQuery(LogEvent logEvent)
        {
            List<KeyValuePair<string, string>> queryString = new List<KeyValuePair<string, string>>();

            // TODO: Extract query parameters

            return queryString;
        }

        /// <summary>
        /// Convert Serilog data to ElmahIO message
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        private static List<KeyValuePair<string, string>> PropertiesToData(LogEvent logEvent)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>();
            if (logEvent.Exception != null)
            {
                data.AddRange(
                    logEvent.Exception.Data.Keys.Cast<object>()
                        .Select(
                            key =>
                                new KeyValuePair<string, string>(key.ToString(), logEvent.Exception.Data[key].ToString())));
            }

            data.AddRange(logEvent.Properties.Select(p => new KeyValuePair<string, string>(p.Key, p.Value.ToString()))); 
            return data;
        }

        /// <summary>
        /// Convert Serilog servirty to ElmahIO serverity
        /// </summary>
        /// <param name="logEvent"></param>
        /// <returns></returns>
        private static Severity LevelToSeverity(LogEvent logEvent)
        {
            switch (logEvent.Level)
            {
                case LogEventLevel.Debug:
                    return Severity.Debug;
                case LogEventLevel.Error:
                    return Severity.Error;
                case LogEventLevel.Fatal:
                    return Severity.Fatal;
                case LogEventLevel.Verbose:
                    return Severity.Verbose;
                case LogEventLevel.Warning:
                    return Severity.Warning;
                default:
                    return Severity.Information;
            }
        }

        /// <summary>
        /// Convert Serilog LogEvent to JSON 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string MessageToJson(Message message)
        {
            string result = JsonConvert.SerializeObject(message);

            return result;
        }        
    }
}