using System;

namespace ExpressionDump.Core
{

    public static class FuncExt
    {
        /// <summary>
        /// Extension method
        /// </summary>
        public static Func<TArg1, Func<TArg2, TReturn>> Curry<TArg1, TArg2, TReturn>(this Func<TArg1, TArg2, TReturn> @this)
        {
            return arg1 => arg2 => @this(arg1, arg2);
        }


        /// <summary>
        /// Extension method
        /// </summary>
        public static Func<TArg1, Func<TArg2, Func<TArg3, TReturn>>> Curry<TArg1, TArg2, TArg3, TReturn>(this Func<TArg1, TArg2, TArg3, TReturn> @this)
        {
            return arg1 => arg2 => arg3 => @this(arg1, arg2, arg3);
        }
    }

}
