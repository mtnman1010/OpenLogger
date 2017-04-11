using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using OpenLogger.Sample.MVVM.Views;

namespace OpenLogger.Sample.MVVM.ViewModels
{
    public class MainViewModel : ViewModel
    {
        private bool stopThreads;

        public MainViewModel(Dispatcher dispatcher)
        {
            StopThreads = false;

            Dispatcher = dispatcher;
            InitializeLogging(true);

            Title = "OpenLogger Sample MVVM";
            LoggerFacade.LogVerbose("Set Title: " + Title);

            StartLoggingCommand = new RelayCommand(StartLogging, CanStartLogging);
            StopLoggingCommand = new RelayCommand(StopLogging, CanStopLogging);
            ExitAppCommand = new RelayCommand(ExitApp);

            TabItems = new ObservableCollection<CustomTabItem>
            {
                new CustomTabItem {Title = "All Logs", Content = GetMasterLoggingView()},
                new CustomTabItem {Title = "Tab 1", Content = GetLoggingView()},
                new CustomTabItem {Title = "Tab 2", Content = GetLoggingView()}
            };
        }

        public ICommand StartLoggingCommand { get; private set; }
        public ICommand StopLoggingCommand { get; private set; }
        public ICommand ExitAppCommand { get; private set; }

        public ObservableCollection<CustomTabItem> TabItems { get; set; }

        private Object obj = new Object();
        public bool StopThreads
        {
            get { return stopThreads; }
            set
            {
                lock (obj)
                {
                    stopThreads = value;
                }
            }
        }

        LoggingView GetMasterLoggingView()
        {
            var loggingView = new LoggingView();
            var loggingViewModel = new LoggingViewModel(Dispatcher, new LoggerFacade(Logger.Instance, "Master Logging View"));
            loggingView.DataContext = loggingViewModel;
            return loggingView;
        }

        LoggingView GetLoggingView()
        {
            var loggingView = new LoggingView();
            
            // Option 1: Pass in LoggerFacade with Group Id
            var loggingViewModel = new LoggingViewModel(Dispatcher, new LoggerFacade{GroupId = LoggerFacade.GetNewGroupId()});
            
            // Option 2: Pass Group Id in via Property Setter
            //if (loggingViewModel is IHaveLoggerGroup)
            //    (loggingViewModel as IHaveLoggerGroup).LogGroupId = LoggerFacade.GetNewGroupId();

            loggingView.DataContext = loggingViewModel;

            return loggingView;
        }

        bool CanStartLogging()
        {
            //return threads.Count == 0;
            return true;
        }

        void StartLogging()
        {
            StopThreads = false;
            foreach (var tabItem in TabItems.Where(tabItem => tabItem.Content.DataContext is IHaveLoggerGroup))
            {
                var groupId = (tabItem.Content.DataContext as IHaveLoggerGroup).LogGroupId;
                threads.Add(new Thread(() => CreateSomeLogs(groupId)));
            }
            foreach (var thread in threads.Where(x => x.ThreadState == ThreadState.Unstarted))
            {
                thread.Start();
            }
        }

        bool CanStopLogging()
        {
            //return threads.Count > 0;
            return true;
        }

        void StopLogging()
        {
            StopThreads = true;
            var stopCounter = 0;

            var task = Task.Factory.StartNew(() =>
            {
                while (threads.Any(x => x.ThreadState == ThreadState.Running))
                {
                    Thread.Sleep(500);
                    if (stopCounter++ >= 10) // 5 seconds
                        break;
                }

                foreach (var thread in threads.Where(x => x.ThreadState == ThreadState.Running))
                {
                    thread.Abort();
                }

                threads.Clear();
            });
        }

        void CreateSomeLogs(int groupId)
        {
            var rand = new Random(groupId);
            LogSeverity severity;

            var logger = new LoggerFacade(Logger.Instance, "Thread [" + Thread.CurrentThread.ManagedThreadId + "]")
            {
                GroupId = groupId
            };

            while (!StopThreads)
            {
                try
                {
                    severity = (LogSeverity) rand.Next(1, 7); // 7 will generate an exception
                    switch (severity)
                    {
                        case LogSeverity.Debug:
                            logger.LogDebug("Debug Message from Group Id: " + groupId);
                            break;
                        case LogSeverity.Fatal:
                            logger.LogFatal("Fatal Message from Group Id: " + groupId);
                            break;
                        case LogSeverity.Error:
                            logger.LogError("Error Message from Group Id: " + groupId);
                            break;
                        case LogSeverity.Warning:
                            logger.LogWarning("Warning Message from Group Id: " + groupId);
                            break;
                        case LogSeverity.Verbose:
                            logger.LogVerbose("Verbose Message from Group Id: " + groupId);
                            break;
                        case LogSeverity.Info:
                            logger.LogInfo("Info Message from Group Id: " + groupId);
                            break;
                        default:
                            logger.LogDebug("Default Message from Group Id: " + groupId);
                            break;
                    }
                }
                catch (ThreadAbortException ex)
                {
                    logger.LogError("Thread was aborted.", ex);
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError("Exception encountered while creating a threaded log entry.", ex);
                    break;
                }

                Thread.Sleep(rand.Next(1000, 3000));
            }

            logger.LogInfo("Thread Cancelled");
        }

        void ExitApp()
        {
            foreach (var thread in threads.Where(thread => thread.ThreadState == ThreadState.Running))
            {
                thread.Abort();
            }
            threads.Clear();

            ViewService.Close();
        }
    }

    public class CustomTabItem
    {
        public string Title { get; set; }
        public UserControl Content { get; set; }
    }
}
