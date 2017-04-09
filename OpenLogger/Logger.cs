using System;
using System.Collections.Generic;

namespace OpenLogger
{
    public sealed class Logger : ILogger
    {
        public delegate void LogEventHandler(object sender, LogEventArgs e);
        public event LogEventHandler Log;

        private static readonly Logger instance = new Logger();
        private LogSeverity severity;
        private bool isDebug;
        private bool isVerbose;
        private bool isInfo;
        private bool isWarning;
        private bool isError;
        private bool isFatal;
        private IEnumerable<LogEventArgs> buffer;

        private Logger()
        {
            // Default LogSeverity
            Severity = LogSeverity.Error;
            Buffer = new FixedConcurrentQueue<LogEventArgs>((Int32.MaxValue) / 2);
        }

        public static Logger Instance
        {
            get { return instance; }
        }

        public LogSeverity Severity
        {
            get { return severity; }
            set
            {
                severity = value;

                // Set booleans to help improve performance
                var tmpSeverity = (int)severity;

                isDebug = ((int)LogSeverity.Debug) >= tmpSeverity;
                isVerbose = ((int)LogSeverity.Verbose) >= tmpSeverity;
                isInfo = ((int)LogSeverity.Info) >= tmpSeverity;
                isWarning = ((int)LogSeverity.Warning) >= tmpSeverity;
                isError = ((int)LogSeverity.Error) >= tmpSeverity;
                isFatal = ((int)LogSeverity.Fatal) >= tmpSeverity;
            }
        }

        public IEnumerable<LogEventArgs> Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        public void LogDebug(string origin, int groupId, string message)
        {
            if (isDebug)
                LogDebug(origin, groupId, message, null);
        }

        public void LogDebug(string origin, int groupId, string message, Exception exception)
        {
            if (isDebug)
                OnLog(new LogEventArgs(LogSeverity.Debug, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogDebug(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isDebug)
                OnLog(new LogEventArgs(LogSeverity.Debug, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void LogVerbose(string origin, int groupId, string message)
        {
            if (isVerbose)
                LogVerbose(origin, groupId, message, null);
        }

        public void LogVerbose(string origin, int groupId, string message, Exception exception)
        {
            if (isVerbose)
                OnLog(new LogEventArgs(LogSeverity.Verbose, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogVerbose(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isVerbose)
                OnLog(new LogEventArgs(LogSeverity.Verbose, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void LogInfo(string origin, int groupId, string message)
        {
            if (isInfo)
                LogInfo(origin, groupId, message, null);
        }

        public void LogInfo(string origin, int groupId, string message, Exception exception)
        {
            if (isInfo)
                OnLog(new LogEventArgs(LogSeverity.Info, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogInfo(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isInfo)
                OnLog(new LogEventArgs(LogSeverity.Info, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void LogWarning(string origin, int groupId, string message)
        {
            if (isWarning)
                LogWarning(origin, groupId, message, null);
        }

        public void LogWarning(string origin, int groupId, string message, Exception exception)
        {
            if (isWarning)
                OnLog(new LogEventArgs(LogSeverity.Warning, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogWarning(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isWarning)
                OnLog(new LogEventArgs(LogSeverity.Warning, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void LogError(string origin, int groupId, string message)
        {
            if (isError)
                LogError(origin, groupId, message, null);
        }

        public void LogError(string origin, int groupId, string message, Exception exception)
        {
            if (isError)
                OnLog(new LogEventArgs(LogSeverity.Error, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogError(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isError)
                OnLog(new LogEventArgs(LogSeverity.Error, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void LogFatal(string origin, int groupId, string message)
        {
            if (isFatal)
                LogFatal(origin, groupId, message, null);
        }

        public void LogFatal(string origin, int groupId, string message, Exception exception)
        {
            if (isFatal)
                OnLog(new LogEventArgs(LogSeverity.Fatal, origin, groupId, message, exception, DateTime.Now, null));
        }

        public void LogFatal(string origin, int groupId, string message, Exception exception, object dataObject)
        {
            if (isFatal)
                OnLog(new LogEventArgs(LogSeverity.Fatal, origin, groupId, message, exception, DateTime.Now, dataObject));
        }

        public void OnLog(LogEventArgs e)
        {
            if (Buffer != null)
                (Buffer as FixedConcurrentQueue<LogEventArgs>).Enqueue(e);
            if (Log != null)
                Log(this, e);
        }

        public void Attach(ILog observer)
        {
            Log += observer.Log;
        }

        public void Detach(ILog observer)
        {
            Log -= observer.Log;
        }
    }
}
