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


        /// <summary>
        /// Extension method
        /// <para>
        /// Takes @this and pipes it into func as an argument. Like a non-variadic apply with @this as the argument.
        /// </para>
        /// </summary>
        internal static TReturn Pipe<T, TReturn>(this T @this, Func<T, TReturn> func)
        {
            return func(@this);
        }
    }

}
