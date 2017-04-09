using System;

namespace OpenLogger
{
    public class LogEventArgs : EventArgs
    {
        public LogEventArgs(LogSeverity severity, string origin, int groupId, string message, Exception exception, DateTime timestamp, object dataObject)
        {
            Severity = severity;
            Origin = origin;
            GroupId = groupId;
            Message = message;
            Exception = exception;
            Timestamp = timestamp;
            DataObject = dataObject;
        }

        public LogSeverity Severity { get; private set; }
        public string Origin { get; private set; }
        public int GroupId { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }
        public DateTime Timestamp { get; private set; }
        public object DataObject { get; private set; }
        public string SeverityString
        {
            get { return Severity.ToString("G"); }
        }

        public override string ToString()
        {
            return Timestamp + ":  " + Origin + " - (" + SeverityString + ") " + Message + Exception.IfNotNull(x => " - " + x.Message) + " [" + DataObject.GetType() + "]";
        }
    }
}
