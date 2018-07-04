using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectValidator
{
    public static class ValidatorBuilderExtensions
    {
        public static ValidatorBuilder AddDescendantFilter<T>(this ValidatorBuilder builder)
        {
            return builder.AddDescendantFilter(propertyInfo =>
            {
                return typeof(T).IsAssignableFrom(propertyInfo.PropertyType);
            });
        }

        public static ValidatorPropertyBuilder<TType> ForType<TType>(this ValidatorBuilder validatorBuilder) => new ValidatorPropertyBuilder<TType>(validatorBuilder);

        public static ValidatorBuilder AddKnownTypes(this ValidatorBuilder builder, Assembly assembly, Func<Type, bool> selector)
        {
            var types = assembly.GetExportedTypes().Where(selector);

            return builder.AddKnownTypes(types);
        }

        public static ValidatorBuilder AddKnownType<T>(this ValidatorBuilder builder) => builder.AddKnownType(typeof(T));

        public static ValidatorBuilder AddKnownTypes<T>(this ValidatorBuilder builder, Assembly assembly)
        {
            return builder.AddKnownTypes(assembly, typeof(T).IsAssignableFrom);
        }

        public static ValidatorBuilder AddRegexConstraint<T>(this ValidatorBuilder builder, Func<T, string> patternConstraint)
            where T : Attribute
        {
            return builder.AddConstraint(propertyInfo =>
            {
                var attribute = propertyInfo.GetCustomAttribute<T>();

                if (attribute == null)
                {
                    return null;
                }

                return new PatternConstraint(propertyInfo, patternConstraint(attribute));
            });
        }
    }
}
