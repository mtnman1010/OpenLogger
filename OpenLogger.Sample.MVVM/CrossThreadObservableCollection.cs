using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using OpenLogger.Sample.MVVM.Extensions;

namespace OpenLogger.Sample.MVVM
{
    public class CrossThreadObservableCollection<T> : ObservableCollection<T>
    {
        readonly Dispatcher dispatcher;

        public CrossThreadObservableCollection(Dispatcher dispatcher, IEnumerable<T> items)
            : base(items)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            this.dispatcher = dispatcher;
        }

        public CrossThreadObservableCollection(Dispatcher dispatcher)
        {
            if (dispatcher == null)
                throw new ArgumentNullException("dispatcher");
            this.dispatcher = dispatcher;
        }

        protected override void ClearItems()
        {
            dispatcher.BeginInvoke(() => base.ClearItems());
        }

        protected override void InsertItem(int index, T item)
        {
            dispatcher.BeginInvoke(() => base.InsertItem(index, item));
        }

        protected override void MoveItem(int oldIndex, int newIndex)
        {
            dispatcher.BeginInvoke(() => base.MoveItem(oldIndex, newIndex));
        }

        protected override void RemoveItem(int index)
        {
            dispatcher.BeginInvoke(() => base.RemoveItem(index));
        }

        protected override void SetItem(int index, T item)
        {
            dispatcher.BeginInvoke(() => base.SetItem(index, item));
        }
    }
}
