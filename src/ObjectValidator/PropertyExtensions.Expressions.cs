using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectValidator
{
    internal static class PropertyExtensions
    {
        public static Func<object, object> GetPropertyDelegate(this PropertyInfo property)
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
    }
}
