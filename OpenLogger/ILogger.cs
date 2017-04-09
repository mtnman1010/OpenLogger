using System;
using System.Collections.Generic;

namespace OpenLogger
{
    public interface ILogger
    {
        LogSeverity Severity { get; set; }
        IEnumerable<LogEventArgs> Buffer { get; }
        void LogDebug(string origin, int groupId, string message);
        void LogDebug(string origin, int groupId, string message, Exception exception);
        void LogDebug(string origin, int groupId, string message, Exception exception, object dataObject);
        void LogVerbose(string origin, int groupId, string message);
        void LogVerbose(string origin, int groupId, string message, Exception exception);
        void LogVerbose(string origin, int groupId, string message, Exception exception, object dataObject);
        void LogInfo(string origin, int groupId, string message);
        void LogInfo(string origin, int groupId, string message, Exception exception);
        void LogInfo(string origin, int groupId, string message, Exception exception, object dataObject);
        void LogWarning(string origin, int groupId, string message);
        void LogWarning(string origin, int groupId, string message, Exception exception);
        void LogWarning(string origin, int groupId, string message, Exception exception, object dataObject);
        void LogError(string origin, int groupId, string message);
        void LogError(string origin, int groupId, string message, Exception exception);
        void LogError(string origin, int groupId, string message, Exception exception, object dataObject);
        void LogFatal(string origin, int groupId, string message);
        void LogFatal(string origin, int groupId, string message, Exception exception);
        void LogFatal(string origin, int groupId, string message, Exception exception, object dataObject);
    }
}
