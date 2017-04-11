using System;
using System.Windows.Threading;

namespace OpenLogger.Sample.MVVM.Extensions
{
    public static class DispatcherExtensions
    {
        public static void BeginInvoke(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher.CheckAccess())
                action();
            else
                dispatcher.BeginInvoke(action);
        }

        public static TResult Invoke<TResult>(this Dispatcher dispatcher, Func<TResult> func)
        {
            if (dispatcher.CheckAccess())
                return func();
            return (TResult)dispatcher.Invoke(func);
        }

        public static void Invoke(this Dispatcher dispatcher, Action func)
        {
            if (dispatcher.CheckAccess())
                func();
            else
                dispatcher.Invoke(func);
        }
    }
}
