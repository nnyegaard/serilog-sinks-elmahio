namespace Serilog.Sinks.ElmahIO
{
    using System;
    using Configuration;
    using Events;
    using Sinks.ElmahIO;

    public static class ElmahIOLoggerConfigurationExtensions
    {
        public static LoggerConfiguration ElmahIO(this LoggerSinkConfiguration sinkConfiguration, Guid logId, LogEventLevel restrictedToMinimumLevel = LevelAlias.Minimum,
            IFormatProvider formatProvider = null)
        {
            if (sinkConfiguration == null)
            {
                throw new ArgumentNullException(nameof(sinkConfiguration));
            }

            ElmahIOSink sink = new ElmahIOSink(formatProvider, logId);
            LoggerConfiguration config = sinkConfiguration.Sink(sink);

            return config;
        }
    }
}