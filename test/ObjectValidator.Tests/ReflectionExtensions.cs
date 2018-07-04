using System;
using System.Reflection;

namespace ObjectValidator
{
    internal static class ReflectionExtensions
    {
#if FEATURE_TYPEINFO
        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetProperty(name);
        }
#endif
    }
}
