using System;
using System.CodeDom;
using System.Collections.Generic;
using Microsoft.CSharp;

namespace ExpressionDump.Core
{

    public static class TypeExt
    {
        static readonly Dictionary<Type, string> aliasDictionary = new Dictionary<Type, string>()
        {
            { typeof(int), "int" },
            { typeof(string), "string" },
        };

        public static string GetTypeAliasOrSelf(this Type @this)
        {
            return aliasDictionary.ContainsKey(@this) ? aliasDictionary[@this] : @this.ToString();
        }
    }

}
