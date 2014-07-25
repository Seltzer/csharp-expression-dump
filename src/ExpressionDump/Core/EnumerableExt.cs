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


        /// <summary>
        /// Extension method
        /// <para>If the specified predicate is satisfied, transform input using the specified transform function. Else, simply return input.</para>
        /// </summary>
        public static IEnumerable<T> If<T>(this IEnumerable<T> source, Func<bool> predicate, Func<IEnumerable<T>, IEnumerable<T>> transformFunc)
        {
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            if (transformFunc == null)
                throw new ArgumentNullException("transformFunc");

            return predicate() ? transformFunc(source) : source;
        }


        
        /// <summary>
        /// Extension method
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> allItemsAction = null, Action inBetweenItemsAction = null)
        {
            var list = source.ToList();

            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0 && inBetweenItemsAction != null)
                    inBetweenItemsAction();

                if (allItemsAction != null)
                    allItemsAction(list[i]);
            }
        }
    }
}
