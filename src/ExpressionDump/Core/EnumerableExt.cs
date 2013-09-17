using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionDump.Core
{
    public static class EnumerableExt
    {
        /// <summary>
        /// Extension method
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T elementToAppend)
        {
            foreach (T element in @this)
                yield return element;

            yield return elementToAppend;
        }
    }
}
