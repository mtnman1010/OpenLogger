using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;

namespace OpenLogger.Sample.MVVM.ViewModels
{
    public class LoggingViewModel : ViewModel, IHaveLoggerGroup, ILog
    {
        public enum LogOrderBy
        {
            Ascending = 0,
            Descending = 1
        }

        private CrossThreadObservableCollection<LogEventArgs> logs;
        private int logGroupId;
        private LogOrderBy logOrder;
        //private LogSeverity selectedLogSeverity;
        private ObservableCollection<LogSeverity> logSeverities;
        private ObservableCollection<LogSeverity> defaultSelectedLogSeverities;
        private ObservableCollection<LogSeverity> selectedLogSeverities;

        public LoggingViewModel(Dispatcher dispatcher, LoggerFacade loggerFacade)
        {
            this.Dispatcher = dispatcher;
            Title = "Logging";

            LoggerFacade = loggerFacade; // Option 1: Pass facade in since Group is defined outside LoggerViewModel
            LoggerFacade.Logger = Logger.Instance;
            logGroupId = loggerFacade.GroupId;
            Logger.Instance.Attach(this);

            logSeverities = new ObservableCollection<LogSeverity>(Enum.GetValues(typeof(LogSeverity)).Cast<LogSeverity>());
            logOrder = LogOrderBy.Ascending;
            defaultSelectedLogSeverities = new ObservableCollection<LogSeverity>(Enum.GetValues(typeof(LogSeverity)).Cast<LogSeverity>()); ;
            RefreshLogCollection();
            LoggerFacade.LogVerbose("Initialized Group Logging");

            LogFilterChangedCommand = new RelayCommand(LogFilterChanged);
            CopyLogItemCommand = new RelayCommand<LogEventArgs>(CopyLogItem);
            CopyAllLogItemsCommand = new RelayCommand(CopyAllLogItems);
        }

        ~LoggingViewModel()
        {
            Logger.Instance.Detach(this);
        }

        public ICommand LogFilterChangedCommand { get; set; }
        public ICommand CopyLogItemCommand { get; set; }
        public ICommand CopyAllLogItemsCommand { get; set; }

        // Option 2: Use IHaveLoggerGroup setter to pass in Log Group Id since Group is defined outside LoggerViewModel
        public int LogGroupId
        {
            get { return logGroupId; }
            set
            {
                if (logGroupId != value)
                {
                    logGroupId = value;
                    RefreshLogCollection();
                }
            }
        }

        public LogOrderBy LogOrder
        {
            get { return logOrder; }
            set
            {
                if (logOrder != value)
                {
                    logOrder = value;
                    RefreshLogCollection();
                }
            }
        }

        public ObservableCollection<LogOrderBy> LogOrdering
        {
            get { return new ObservableCollection<LogOrderBy>(Enum.GetValues(typeof(LogOrderBy)).Cast<LogOrderBy>()); }
        }

        public ObservableCollection<LogSeverity> LogSeverities
        {
            get
            {
                return logSeverities;
            }
        }

        public ObservableCollection<LogSeverity> DefaultSelectedLogSeverities
        {
            get
            {
                return defaultSelectedLogSeverities;
            }
            set
            {
                defaultSelectedLogSeverities = value;
                RefreshLogCollection();
            }
        }

        public CrossThreadObservableCollection<LogEventArgs> Logs
        {
            get { return logs; }
            set { Update(() => Logs, ref logs, value, false); }
        }

        public void Log(object sender, LogEventArgs e)
        {
            if (LogGroupId == 0)
            {
                if (logOrder == LogOrderBy.Ascending)
                    Logs.Add(e); // Add to bottom (must scroll)
                else
                    Logs.Insert(0, e); // Add to top (visible)
            }
            else if (e.GroupId == LogGroupId)
            {
                if (logOrder == LogOrderBy.Ascending)
                    Logs.Add(e); // Add to bottom (must scroll)
                else
                    Logs.Insert(0, e); // Add to top (visible)
            }
            //RefreshLogCollection();
        }

        private void LogFilterChanged()
        {
            RefreshLogCollection();
        }

        private void CopyLogItem(LogEventArgs logEventArg)
        {
            Clipboard.SetText(FormatLogEventArgString(logEventArg));
        }

        private void CopyAllLogItems()
        {
            var sb = new StringBuilder();
            foreach (var logEventArg in Logs)
            {
                sb.Append(FormatLogEventArgString(logEventArg));
                sb.Append(Environment.NewLine);
            }
            Clipboard.SetText(sb.ToString());
        }

        private void RefreshLogCollection()
        {
            if (LogGroupId == 0)
            {
                Logs = LogOrder == LogOrderBy.Ascending
                    ? new CrossThreadObservableCollection<LogEventArgs>(Dispatcher,
                        LoggerFacade.Logger.Buffer.Where(x => defaultSelectedLogSeverities.Contains(x.Severity)))
                    : new CrossThreadObservableCollection<LogEventArgs>(Dispatcher,
                        LoggerFacade.Logger.Buffer.Where(x => defaultSelectedLogSeverities.Contains(x.Severity))
                            .OrderByDescending(x => x.Timestamp));

            }
            else
            {
                Logs = LogOrder == LogOrderBy.Ascending
                    ? new CrossThreadObservableCollection<LogEventArgs>(Dispatcher,
                        LoggerFacade.Logger.Buffer.Where(x => defaultSelectedLogSeverities.Contains(x.Severity) && x.GroupId == LogGroupId))
                    : new CrossThreadObservableCollection<LogEventArgs>(Dispatcher,
                        LoggerFacade.Logger.Buffer.Where(x => defaultSelectedLogSeverities.Contains(x.Severity) && x.GroupId == LogGroupId)
                            .OrderByDescending(x => x.Timestamp));
            }
        }

        private string FormatLogEventArgString(LogEventArgs logEventArg)
        {
            var sb = new StringBuilder();
            sb.Append(string.Format("{0} - [{1}] ({2}) {3}", logEventArg.SeverityString,
                logEventArg.Timestamp, logEventArg.Origin, logEventArg.Message));
            sb.Append(FormatLogEventArgExceptionString(logEventArg.Exception));
            return sb.ToString();
        }

        private string FormatLogEventArgExceptionString(Exception ex)
        {
            if (ex == null)
                return string.Empty;
            var sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append("Exception: " + ex.Message + Environment.NewLine);
            sb.Append("Stack Trace: " + ex.StackTrace);
            sb.Append(FormatLogEventArgExceptionString(ex.InnerException));
            return sb.ToString();
        }
    }
}
