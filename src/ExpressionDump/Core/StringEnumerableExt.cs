using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionDump.Core
{

    public static class StringEnumerableExt
    {
        /// <summary>
        /// Extension method
        /// <para>
        /// Instancey IEnumerable wrapper for String.Join
        /// </para>
        /// </summary>
        public static string StringJoin(this IEnumerable<string> list, string separator = "")
        {
            return String.Join(separator, list.ToArray());
        }
    }

}
