namespace Serilog.Sinks.ElmahIO.Sinks.ElmahIO
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Representation of ElmahIO message in .NET
    /// </summary>
    public class Message
    {
        public string id { get; set; }

        public string Application { get; set; }

        public string Detail { get; set; }

        public string hostname { get; set; }

        public string title { get; set; }

        public string Source { get; set; }

        public int? StatusCode { get; set; }

        public DateTime DateTime { get; set; }

        public string Type { get; set; }

        public string User { get; set; }

        public Severity? Severity { get; set; }

        public string url { get; set; }

        public List<KeyValuePair<string, string>> Cookies { get; set; }

        public List<KeyValuePair<string, string>> Form { get; set; }

        public List<KeyValuePair<string, string>> QueryString { get; set; }

        public List<KeyValuePair<string, string>> ServerVariables { get; set; }

        public List<KeyValuePair<string, string>> Data { get; set; }

        public Message(string title)
        {
            this.title = title;
        }
    }
}