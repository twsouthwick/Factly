using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ObjectValidator
{
    internal static class PropertyExtensions
    {
        public static Func<object, object> GetPropertyDelegate(this PropertyInfo property)
        {
            return property.GetValue;
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties;
        }

        public static bool IsAssignableFrom(this Type type, Type other)
        {
            return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }
    }
}
