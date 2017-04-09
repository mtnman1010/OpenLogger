using System;

namespace OpenLogger
{
    public class LoggerFacade
    {
        private static int currentGroupId = 0;

        public LoggerFacade()
        {
            GroupId = 0;
        }

        public LoggerFacade(string origin)
            : this(null, origin)
        {
        }

        public LoggerFacade(ILogger logger, string origin)
        {
            Logger = logger;
            Origin = origin;
            GroupId = 0;
        }

        public ILogger Logger { get; set; }
        public string Origin { get; set; }
        public int GroupId { get; set; }

        public static int GetNewGroupId()
        {
            return ++currentGroupId;
        }

        public void LogDebug(string message)
        {
            if (Logger != null)
                Logger.LogDebug(Origin, GroupId, message);
        }

        public void LogDebug(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogDebug(Origin, GroupId, message, exception);
        }

        public void LogDebug(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogDebug(Origin, GroupId, message, exception, dataObject);
        }

        public void LogVerbose(string message)
        {
            if (Logger != null)
                Logger.LogVerbose(Origin, GroupId, message);
        }

        public void LogVerbose(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogVerbose(Origin, GroupId, message, exception);
        }

        public void LogVerbose(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogVerbose(Origin, GroupId, message, exception, dataObject);
        }

        public void LogInfo(string message)
        {
            if (Logger != null)
                Logger.LogInfo(Origin, GroupId, message);
        }

        public void LogInfo(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogInfo(Origin, GroupId, message, exception);
        }

        public void LogInfo(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogInfo(Origin, GroupId, message, exception, dataObject);
        }

        public void LogWarning(string message)
        {
            if (Logger != null)
                Logger.LogWarning(Origin, GroupId, message);
        }

        public void LogWarning(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogWarning(Origin, GroupId, message, exception);
        }

        public void LogWarning(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogWarning(Origin, GroupId, message, exception, dataObject);
        }

        public void LogError(string message)
        {
            if (Logger != null)
                Logger.LogError(Origin, GroupId, message);
        }

        public void LogError(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogError(Origin, GroupId, message, exception);
        }

        public void LogError(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogError(Origin, GroupId, message, exception, dataObject);
        }

        public void LogFatal(string message)
        {
            if (Logger != null)
                Logger.LogFatal(Origin, GroupId, message);
        }

        public void LogFatal(string message, Exception exception)
        {
            if (Logger != null)
                Logger.LogFatal(Origin, GroupId, message, exception);
        }

        public void LogFatal(string message, Exception exception, object dataObject)
        {
            if (Logger != null)
                Logger.LogFatal(Origin, GroupId, message, exception, dataObject);
        }
    }
}
