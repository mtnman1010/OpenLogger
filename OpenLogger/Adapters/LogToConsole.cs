using System;

namespace OpenLogger.Adapters
{
    public class LogToConsole : ILog
    {
        public void Log(object sender, LogEventArgs e)
        {
            var message = e.SeverityString + " - [" + e.Timestamp.ToString() + "] (" + e.Origin + ") " + e.Message;
            Console.WriteLine(message);
        }
    }
}
