using System;

namespace ExpressionDump.Core
{

    public static class ObjectExt
    {
        /// <summary>
        /// Extension method
        /// </summary>
        public static TReturn IfNotNull<T, TReturn>(this T @this, Func<T, TReturn> func)
            where T : class
            where TReturn : class
        {
            if (func == null)
                throw new ArgumentNullException("func");

            return @this != null ? func(@this) : null;
        }
    }

}
