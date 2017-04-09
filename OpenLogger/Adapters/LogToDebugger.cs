using System;
using System.Diagnostics;
using System.Globalization;

namespace OpenLogger.Adapters
{
    public class LogToDebugger : ILog
    {
        public void Log(object sender, LogEventArgs e)
        {
            var message = e.SeverityString + " - [" + e.Timestamp.ToString(CultureInfo.InvariantCulture) + "] (" +
                          e.Origin + ") " + e.Message;
            Debugger.Log(0, null, message + Environment.NewLine + Environment.NewLine);
        }
    }
}
