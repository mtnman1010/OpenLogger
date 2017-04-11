using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using OpenLogger.Adapters;
using OpenLogger.Sample.MVVM.ViewModels;

namespace OpenLogger.Sample.MVVM
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private LoggerFacade Log { get; set; }

        public App()
        {
#if DEBUG
            Logger.Instance.Severity = LogSeverity.Debug;
            if (Debugger.IsAttached)
                Logger.Instance.Attach(new LogToDebugger());
#else
            Logger.Instance.Severity = LogSeverity.Verbose; // Maybe get from application settings.
#endif
            DispatcherUnhandledException += HandleException;
            AppDomain.CurrentDomain.UnhandledException += HandleException;

            InitializeLogging(null, "OpenLogger Sample MVVM");

            var mainViewModel = new MainViewModel(Dispatcher);

            var mainView = new MainView(Dispatcher.CurrentDispatcher, mainViewModel);

            MainWindow = mainView;
            MainWindow.Show();
        }

        private void HandleException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
                e.Handled = true;
        }

        private void HandleException(object sender, UnhandledExceptionEventArgs e)
        {
            if (!Debugger.IsAttached)
            {
                var exception = (Exception)e.ExceptionObject;
                Log.LogFatal("Unhandled Exception Encountered", exception);
                // Display custom exception window
            }
        }

        private void InitializeLogging(ILog[] logs, string origin)
        {
            Log = new LoggerFacade(Logger.Instance, origin);

            if (logs != null)
            {
                foreach (var log in logs)
                {
                    Logger.Instance.Attach(log);
                }
            }
        }
    }
}
