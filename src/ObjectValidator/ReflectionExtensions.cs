using System;
using System.Reflection;

#if FEATURE_REFEMIT
using System.Reflection.Emit;
#else
using System.Collections.Generic;
using System.Linq.Expressions;
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
            var isValueType = property.PropertyType.IsValueType;
            var callingClass = property.DeclaringType;
            var propertyType = isValueType ? typeof(object) : property.PropertyType;
            var method = new DynamicMethod("ValidatorFastPropertyGetter", propertyType, new[] { typeof(object) }, callingClass, true);

            var il = method.GetILGenerator();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(callingClass.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, callingClass);
            il.Emit(OpCodes.Call, property.GetGetMethod(true));
            il.Emit(OpCodes.Ret);

            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }
#else
        {
            var method = property.GetMethod;
            var isValueType = property.DeclaringType.GetTypeInfo().IsValueType;
            var instance = Expression.Parameter(typeof(object), "instance");
            var instanceCast = !isValueType ?
                Expression.TypeAs(instance, property.DeclaringType) :
                Expression.Convert(instance, property.DeclaringType);
            return Expression.Lambda<Func<object, object>>(
                Expression.TypeAs(
                    Expression.Call(instanceCast, method),
                    typeof(object)),
                instance)
                .Compile();
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

        public static IEnumerable<Type> GetExportedTypes(this Assembly assembly)
        {
            return assembly.ExportedTypes;
        }
#else
        public static Type GetTypeInfo(this Type type) => type;
#endif

#if !FEATURE_CUSTOMATTRIBUTE
        public static T GetCustomAttribute<T>(this PropertyInfo property)
            where T : Attribute
        {
            return (T)Attribute.GetCustomAttribute(property, typeof(T));
        }
#endif
    }
}
