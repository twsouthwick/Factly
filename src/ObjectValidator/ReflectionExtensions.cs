using System;
using System.Reflection;

#if FEATURE_REFEMIT
using System.Reflection.Emit;
#elif FEATURE_PROPERTYINFO_GETGETMETHOD
using System.Collections.Generic;
using System.Linq.Expressions;
#else
using System.Collections.Generic;
#endif

namespace ObjectValidator
{
    internal static class ReflectionExtensions
    {
        /// <summary>
        /// Retrieves a delegate to access the value of the property. Some platforms allow for more efficient implementations than others
        /// </summary>
        /// <param name="property">Property to retrieve getter</param>
        /// <returns>A delegate to access the value of the property</returns>
        public static Func<object, object> GetPropertyDelegate(this PropertyInfo property)
#if FEATURE_REFEMIT
        {
            var callingClass = property.DeclaringType;
            var method = new DynamicMethod("ValidatorFastPropertyGetter", property.PropertyType, new[] { typeof(object) }, callingClass, true);

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, callingClass);
            il.Emit(OpCodes.Callvirt, property.GetGetMethod(true));
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }
#elif FEATURE_PROPERTYINFO_GETGETMETHOD
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast =
                !property.DeclaringType.IsValueType ?
                    Expression.TypeAs(instance, property.DeclaringType) :
                    Expression.Convert(instance, property.DeclaringType);
            return Expression.Lambda<Func<object, object>>(
                    Expression.TypeAs(
                        Expression.Call(instanceCast, property.GetGetMethod(nonPublic: true)),
                        typeof(object)),
                    instance)
                .Compile();
        }
#else
        {
            return property.GetValue;
        }
#endif

#if FEATURE_TYPEINFO
        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties;
        }

        public static bool IsAssignableFrom(this Type type, Type other)
        {
            return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
        }
#endif
    }
}
