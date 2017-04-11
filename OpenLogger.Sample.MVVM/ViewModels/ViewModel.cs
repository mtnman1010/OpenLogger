using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using System.Windows.Threading;
using ThreadState = System.Threading.ThreadState;

namespace OpenLogger.Sample.MVVM.ViewModels
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private bool isDirty;
        private bool isBusy;
        static readonly ICollection<ViewModel> viewModels = new List<ViewModel>();
        protected ICollection<Thread> threads;

        public ViewModel()
        {
            viewModels.Add(this);
            threads = new Collection<Thread>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IViewService ViewService { get; set; }
        public Dispatcher Dispatcher { get; set; }

        public string Title { get; set; }
        public LoggerFacade LoggerFacade { get; set; }
        
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set { Update(() => IsDirty, ref isDirty, value, false); }
        }

        protected void InitializeLogging(bool isNewGroup)
        {
            InitializeLogging(isNewGroup, GetType().Name);
        }

        protected void InitializeLogging(bool isNewGroup, string origin)
        {
            LoggerFacade = new LoggerFacade(Logger.Instance, origin);
            if (isNewGroup)
                LoggerFacade.GroupId = LoggerFacade.GetNewGroupId();
        }

        protected void InitializeLogging(LoggerFacade facade)
        {
            LoggerFacade = facade;
        }
        
        protected void Update<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue, bool setDirty)
        {
            if (!Equals(field, newValue))
            {
                field = newValue;
                NotifyPropertyChanged(propertyExpression);
                IsDirty |= setDirty;
            }
        }

        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (Dispatcher != null)
                Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
        }

        protected void NotifyPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var property = (MemberExpression)propertyExpression.Body;
            ValidatePropertyChangedExpression(propertyExpression, property);
            NotifyPropertyChanged(property.Member.Name);
        }

        [Conditional("DEBUG")]
        void ValidatePropertyChangedExpression<TProperty>(Expression<Func<TProperty>> propertyExpression,
            MemberExpression property)
        {
            var expression = property.Expression;
            var constant =
                (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked
                    ? ((UnaryExpression)expression).Operand
                    : expression) as ConstantExpression;
            if (property == null || !(property.Member is PropertyInfo) || !(constant != null && constant.Value == this))
            {
                throw new ArgumentException(
                    string.Format("Expression must be of the form 'this.PropertyName'. Invalid expression '{0}',",
                        propertyExpression));
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
