using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenLogger.Adapters;

namespace OpenLogger.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var loggerInstance = Logger.Instance;
            loggerInstance.Severity = LogSeverity.Debug;
            loggerInstance.Attach(new LogToDebugger());
            loggerInstance.Attach(new LogToConsole());

            var logger = new LoggerFacade(loggerInstance, "Sample Console");

            var threads = new List<Thread>(5);
            for (var i = 0; i < threads.Capacity; i++)
            {
                threads.Add(new Thread(LogThread));
            }

            foreach (var thread in threads)
            {
                try
                {
                    thread.Start();
                    logger.LogVerbose("Started Thread Id: " + thread.ManagedThreadId);
                    while (!thread.IsAlive);
                    Thread.Sleep(750);
                }
                catch (Exception ex)
                {
                    logger.LogFatal("Unable to start thread with index " + threads.IndexOf(thread), ex);
                }
            }

            Thread.Sleep(2000);

            foreach (var thread in threads.Where(x => x.IsAlive))
            {
                logger.LogVerbose("Aborting Thread Id: " + thread.ManagedThreadId);
                thread.Abort();
                logger.LogVerbose("Waiting for Thread Id: " + thread.ManagedThreadId);
                thread.Join();
            }

            Console.ReadKey();
        }

        static void LogThread()
        {
            var logger = new LoggerFacade(Logger.Instance, "Thread Id: " + Thread.CurrentThread.ManagedThreadId);
            while (true)
            {
                logger.LogVerbose("Hello World!");
                Thread.Sleep(500);
            }
        }
    }
}
