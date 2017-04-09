using System;
using System.Diagnostics;
using System.Globalization;

namespace OpenLogger.Adapters
{
    public class LogToEventLog : ILog
    {
        private readonly string source;
        private readonly string log;

        public LogToEventLog(string source, string log)
            : this(source, 0, log)
        {
        }

        /// <summary>
        /// Log to the System EventLog
        /// </summary>
        /// <param name="source">Application Name</param>
        /// <param name="log">EventLog to log too</param>
        public LogToEventLog(string source, int groupId, string log)
        {
            this.source = source;
            this.log = log;

            // TODO: Test that logging is written to event log and not application log

            try
            {
                if (!EventLog.SourceExists(log))
                {
                    if (EventLog.SourceExists(source))
                    {
                        EventLog.DeleteEventSource(source);
                        Logger.Instance.LogInfo("LogToEventLog", groupId, "Removing existing Source: " + source);
                    }
                }
                Logger.Instance.LogInfo("LogToEventLog", groupId, "Creating EventLog: " + log);
                EventLog.CreateEventSource(new EventSourceCreationData(source, log));
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("LogToEventLog", groupId, "Unable to create EventLog: " + ex.Message + " StackTrace: " + ex.StackTrace);
            }
        }

        public void Log(object sender, LogEventArgs e)
        {
            var message = e.SeverityString + " - [" + e.Timestamp.ToString(CultureInfo.InvariantCulture) + "] (" +
                          e.Origin + ") " + e.Message;

            // This is a little bit like error hiding but...
            try
            {
                if (!EventLog.SourceExists(source))
                    EventLog.CreateEventSource(source, log);

                var logEntryType = EventLogEntryType.Information;
                switch (Logger.Instance.Severity)
                {
                    case LogSeverity.Debug:
                        logEntryType = EventLogEntryType.Information;
                        break;
                    case LogSeverity.Info:
                        logEntryType = EventLogEntryType.Information;
                        break;
                    case LogSeverity.Warning:
                        logEntryType = EventLogEntryType.Warning;
                        break;
                    case LogSeverity.Error:
                        logEntryType = EventLogEntryType.Error;
                        break;
                    case LogSeverity.Fatal:
                        logEntryType = EventLogEntryType.Error;
                        break;
                }


                EventLog.WriteEntry(source, message, logEntryType);
            }
            catch (Exception)
            {
                // Suppress
            }
        }
    }
}
