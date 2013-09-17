using System.Reflection;
using System.Runtime.CompilerServices;

namespace ExpressionDump.Core
{

    public static class ReflectionExt
    {
        public static bool IsExtensionMethod(this MethodInfo methodInfo)
        {
            return methodInfo.IsDefined(typeof (ExtensionAttribute), false);
        }
    }

}
