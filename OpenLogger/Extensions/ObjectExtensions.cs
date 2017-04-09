using System;

namespace OpenLogger.Extensions
{
    public static class ObjectExtensions
    {
        public static TResult IfNotNull<T, TResult>(this T value, Func<T, TResult> map)
        {
            if (map == null)
                throw new ArgumentNullException("map");
            return Equals(value, default(T))
                       ? default(TResult)
                       : map(value);
        }
    }
}
