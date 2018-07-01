using System;
using System.Linq;
using System.Reflection;

namespace XmlSchemaValidator
{
    public static class ValidatorBuilderExtensions
    {
        public static ValidatorBuilder WithDescendants<T>(this ValidatorBuilder builder)
        {
            return builder.WithDescendents(propertyInfo =>
            {
                return typeof(T).IsAssignableFrom(propertyInfo.PropertyType);
            });
        }

        public static ValidatorBuilder AddKnownTypes(this ValidatorBuilder builder, Assembly assembly, Func<Type, bool> selector)
        {
            var types = assembly.ExportedTypes.Where(selector);

            return builder.AddKnownTypes(types);
        }

        public static ValidatorBuilder AddKnownType<T>(this ValidatorBuilder builder) => builder.AddKnownType(typeof(T));

        public static ValidatorBuilder AddKnownTypes<T>(this ValidatorBuilder builder, Assembly assembly)
        {
            return builder.AddKnownTypes(assembly, typeof(T).IsAssignableFrom);
        }
    }
}
