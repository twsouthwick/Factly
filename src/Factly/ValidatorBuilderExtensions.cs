// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// A collection of extension methods for helper functions for <see cref="ValidatorBuilder"/>.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// Adds a property filter for any property that has a return type that derives from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of property to find.</typeparam>
        /// <param name="builder">The current <see cref="ValidatorBuilder"/>.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddPropertyFilter<T>(this ValidatorBuilder builder)
        {
            return builder.AddPropertyFilter(propertyInfo =>
            {
                return typeof(T).IsAssignableFrom(propertyInfo.PropertyType);
            });
        }

        /// <summary>
        /// Creates a property builder for <typeparamref name="TType"/>. If <typeparamref name="TType"/> is unknown,
        /// it will be added to the known types of the <paramref name="validatorBuilder"/>.
        /// </summary>
        /// <typeparam name="TType">Type to add property filters.</typeparam>
        /// <param name="validatorBuilder">The current <see cref="ValidatorBuilder"/>.</param>
        /// <returns>A <see cref="ValidatorPropertyBuilder{TType}"/> for <typeparamref name="TType"/>.</returns>
        public static ValidatorPropertyBuilder<TType> ForType<TType>(this ValidatorBuilder validatorBuilder) => new ValidatorPropertyBuilder<TType>(validatorBuilder);

        /// <summary>
        /// Adds known types from an <see cref="Assembly"/> given a <paramref name="selector"/>.
        /// </summary>
        /// <param name="builder">Current <see cref="ValidatorBuilder"/>.</param>
        /// <param name="assembly">Assembly to search for exported types.</param>
        /// <param name="selector">Type selector.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddKnownTypes(this ValidatorBuilder builder, Assembly assembly, Func<Type, bool> selector)
        {
            var types = assembly.GetExportedTypes().Where(selector);

            return builder.AddKnownTypes(types);
        }

        /// <summary>
        /// Add a known type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to add.</typeparam>
        /// <param name="builder">Current <see cref="ValidatorBuilder"/>.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddKnownType<T>(this ValidatorBuilder builder) => builder.AddKnownType(typeof(T));

        /// <summary>
        /// Add types from <see cref="Assembly"/> that are assignable from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Base type to search for.</typeparam>
        /// <param name="builder">Current <see cref="ValidatorBuilder"/>.</param>
        /// <param name="assembly">Assembly to search.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddKnownTypes<T>(this ValidatorBuilder builder, Assembly assembly)
        {
            return builder.AddKnownTypes(assembly, typeof(T).IsAssignableFrom);
        }

        /// <summary>
        /// Adds a constraint for regular expressions based on a supplied attribute.
        /// </summary>
        /// <typeparam name="T">Attribute that identifies a regular expression constraint.</typeparam>
        /// <param name="builder">Current <see cref="ValidatorBuilder"/>.</param>
        /// <param name="stringSelector">Selector to retrieve <see cref="string"/> from <typeparamref name="T"/>.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddRegexConstraint<T>(this ValidatorBuilder builder, Func<T, string> stringSelector)
            where T : Attribute
        {
            return builder.AddAttributeConstraint<T>((attribute, propertyInfo) =>
                new PatternConstraint(propertyInfo, builder, stringSelector(attribute)));
        }

        /// <summary>
        /// Adds constraints when the property is annotated with <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute.</typeparam>
        /// <param name="builder">Current <see cref="ValidatorBuilder"/>.</param>
        /// <param name="factory">The factory to create a constraint given an attribute instance.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddAttributeConstraint<TAttribute>(this ValidatorBuilder builder, Func<TAttribute, PropertyInfo, IConstraint> factory)
            where TAttribute : Attribute
        {
            return builder.AddConstraint(propertyInfo =>
            {
                var attribute = propertyInfo.GetCustomAttribute<TAttribute>();

                if (attribute == null)
                {
                    return null;
                }

                return factory(attribute, propertyInfo);
            });
        }
    }
}
