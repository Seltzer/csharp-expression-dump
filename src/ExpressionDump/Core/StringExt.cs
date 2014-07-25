using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionDump.Core
{

    public static class StringExt
    {
        /// <summary>
        /// Extension method
        /// <para>
        /// This is a version of the built-in String.Contains method which allows you to specify a comparison type.
        /// </para>
        /// </summary>
        public static bool Contains(this string str, string substring, StringComparison comparisonType)
        {
            return str.IndexOf(substring, comparisonType) != -1;
        }


        /// <summary>
        /// Extension method
        /// <para>
        /// FIXME: Inconsistent with <see cref="UpToAndIncludingLast"/>
        /// </para>
        /// </summary>
        /// <returns>
        /// Everything after the last instance of <see cref="lastWhat"/> or an empty string if <see cref="lastWhat"/> is not in @this
        /// </returns>
        public static string EverythingAfterLast(this string @this, string lastWhat, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");
            if (lastWhat == null)
                throw new ArgumentNullException("lastWhat");
            
            if (lastWhat == "" || !@this.Contains(lastWhat, stringComparison))
                return "";
            else
                return @this.Substring(@this.UpToAndIncludingLast(lastWhat, stringComparison).Length);
        }
        
        
        /// <summary>
        /// Extension method
        /// </summary>
        /// <exception cref="ArgumentException">If this string does not contain <see cref="lastWhat"/></exception>
        public static string UpToAndIncludingLast(this string @this, string lastWhat, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");
            if (lastWhat == null)
                throw new ArgumentNullException("lastWhat");

            if (lastWhat == "")
                return @this;
            
            int index = @this.LastIndexOf(lastWhat, stringComparison);
            if (index == -1)
                throw new ArgumentException("Specified string (@this) does not contain specified segment (lastWhat)");

            IEnumerable<string> components = @this.BisectBefore(index);
            
            return components.First() + components.Second().Inits().First(init => init.Equals(lastWhat, stringComparison));
        }
        

        /// <summary>
        /// Extension method
        /// </summary>
        /// <exception cref="ArgumentException">If this string does not contain <see cref="lastWhat"/></exception>
        public static string UpToButExcludingLast(this string @this, string lastWhat, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (@this == null)
                throw new ArgumentNullException("@this");
            if (lastWhat == null)
                throw new ArgumentNullException("lastWhat");

            if (lastWhat == "")
                return @this;
            
            int index = @this.LastIndexOf(lastWhat, stringComparison);
            if (index == -1)
                throw new ArgumentException("Specified string (@this) does not contain specified segment (lastWhat)");

            return @this.Substring(0, index);
        }
        
        
        /// <summary>
        /// Extension method
        /// </summary>
        public static IEnumerable<string> BisectBefore(this string @this, int indexToBisectBefore)
        {
            if (indexToBisectBefore < 0)
                throw new ArgumentOutOfRangeException("indexToBisectBefore");
            if (indexToBisectBefore > @this.Length)
                throw new ArgumentOutOfRangeException("indexToBisectBefore");

            string first = @this.Substring(0, indexToBisectBefore);
            string second = @this.Substring(indexToBisectBefore);

            return new[] { first, second };
        }
        
        
        /// <summary>
        /// Datacom extension
        /// <para>
        /// Similar to Haskell inits (http://hackage.haskell.org/package/base-4.6.0.1/docs/Data-List.html#v:inits).
        /// Returns the inits of a string - e.g. for "abc", returns "", "a", "ab", "abc". 
        /// </para>
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Inits(this string @this)
        {
            for (int i = 0; i <= @this.Length; i++)
                yield return @this.Substring(0, i);
        }
    }
}
