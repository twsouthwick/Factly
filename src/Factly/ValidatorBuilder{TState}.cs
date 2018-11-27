// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

#if FEATURE_PARALLEL
using System.Threading.Tasks;
#endif

namespace Factly
{
    /// <summary>
    /// Builder to generate a <see cref="Validator{TState}"/>.
    /// </summary>
    /// <typeparam name="TState">Custom type supplied for the validation.</typeparam>
    public sealed class ValidatorBuilder<TState>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorBuilder{TState}"/> class.
        /// </summary>
        public ValidatorBuilder()
        {
            Types = new HashSet<Type>();
            Constraints = new List<ConstraintBuilder<TState>>();
            PropertyFilters = new List<Func<PropertyInfo, bool>>();
        }

        internal List<Func<PropertyInfo, bool>> PropertyFilters { get; }

        internal List<ConstraintBuilder<TState>> Constraints { get; }

        internal HashSet<Type> Types { get; }

        /// <summary>
        /// Adds a constraint generator that depends on an input <see cref="PropertyInfo"/>.
        /// </summary>
        /// <typeparam name="T">Type of the constraint.</typeparam>
        /// <param name="constraintGenerator">The generator to create a <see cref="IConstraint{TState}"/>.</param>
        /// <returns>A builder instance for the constraint.</returns>
        public ConstraintBuilder<TState, T> AddConstraint<T>(Func<PropertyInfo, BuilderContext<TState>, IConstraint<TState>> constraintGenerator)
            => AddConstraint(new ConstraintBuilder<TState, T>(constraintGenerator));

        /// <summary>
        /// Adds a constraint generator for an input <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Type of the input object to constrain.</typeparam>
        /// <param name="constraint">The constraint function.</param>
        /// <param name="constraintId">The id for the constraint.</param>
        /// <returns>A builder instance for the constraint.</returns>
        public ConstraintBuilder<TState, T> AddConstraint<T>(Action<T, ConstraintContext<TState>> constraint, string constraintId)
        {
            IConstraint<TState> Generator(Type type, BuilderContext<TState> ctx)
            {
                if (type == typeof(T))
                {
                    return new DelegateConstraint<TState, T>(constraint, constraintId);
                }
                else
                {
                    return null;
                }
            }

            AddPropertyFilter<T>();

            AddConstraint((property, ctx) =>
            {
                if (typeof(IEnumerable<T>).IsAssignableFrom(property.PropertyType))
                {
                    return ExpansionConstraint<TState>.Instance;
                }

                return null;
            });

            return AddConstraint(new ConstraintBuilder<TState, T>(Generator));
        }

        /// <summary>
        /// Adds a constraint generator that depends on an input <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="constraintGenerator">The generator to create a <see cref="IConstraint{TState}"/>.</param>
        /// <returns>A builder instance for the constraint.</returns>
        public ConstraintBuilder<TState> AddConstraint(Func<PropertyInfo, BuilderContext<TState>, IConstraint<TState>> constraintGenerator)
        {
            var builder = new ConstraintBuilder<TState>(constraintGenerator);
            Constraints.Add(builder);
            return builder;
        }

        /// <summary>
        /// Add a filter to identify properties that should be traversed.
        /// </summary>
        /// <param name="filter">A filter for a <see cref="PropertyInfo"/>.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddPropertyFilter(Func<PropertyInfo, bool> filter)
        {
            PropertyFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Add a collection of known types to the validator.
        /// </summary>
        /// <param name="types">Types to add.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AddKnownType(type);
            }

            return this;
        }

        /// <summary>
        /// Add a known type to the builder.
        /// </summary>
        /// <param name="type">Type to add.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddKnownType(Type type)
        {
            Types.Add(type);
            return this;
        }

#if FEATURE_PARALLEL
        /// <summary>
        /// Build the <see cref="Validator{TState}"/> from the current information in <see cref="ValidatorBuilder{TState}"/>.
        /// </summary>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns><see cref="Validator{TState}"/> built from <see cref="ValidatorBuilder{TState}"/>.</returns>
        public async Task<Validator<TState>> BuildAsync(CancellationToken token = default)
        {
            if (Types.Count == 0)
            {
                throw new ValidatorBuilderException(SR.MustDeclareTypes, Errors.NoTypes, null, null);
            }

            using (var builder = new BuilderContext<TState>(this, threadSafe: true))
            {
                await Types.TraverseAsync(builder.AddItem, Environment.ProcessorCount, token).ConfigureAwait(false);

                return builder.Get();
            }
        }
#endif

#if !NO_CANCELLATION_TOKEN
        /// <summary>
        /// Build the <see cref="Validator{TState}"/> from the current information in <see cref="ValidatorBuilder{TState}"/>.
        /// </summary>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns><see cref="Validator{TState}"/> built from <see cref="ValidatorBuilder{TState}"/>.</returns>
        public Validator<TState> Build(CancellationToken token = default)
        {
#else
        /// <summary>
        /// Build the <see cref="Validator{TState}"/> from the current information in <see cref="ValidatorBuilder{TState}"/>.
        /// </summary>
        /// <returns><see cref="Validator{TState}"/> built from <see cref="ValidatorBuilder{TState}"/>.</returns>
        public Validator<TState> Build()
        {
            var token = default(CancellationToken);
#endif
            if (Types.Count == 0)
            {
                throw new ValidatorBuilderException(SR.MustDeclareTypes, Errors.NoTypes, null, null);
            }

            using (var builder = new BuilderContext<TState>(this, threadSafe: false))
            {
                Types.Traverse(builder.AddItem, token);

                return builder.Get();
            }
        }

        /// <summary>
        /// Add a known type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to add.</typeparam>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddKnownType<T>() => AddKnownType(typeof(T));

        /// <summary>
        /// Add types from <see cref="Assembly"/> that are assignable from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Base type to search for.</typeparam>
        /// <param name="assembly">Assembly to search.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddKnownTypes<T>(Assembly assembly)
            => AddKnownTypes(assembly, typeof(T).IsAssignableFrom);

        /// <summary>
        /// Adds a constraint for regular expressions based on a supplied attribute.
        /// </summary>
        /// <typeparam name="TAttribute">Attribute that identifies a regular expression constraint.</typeparam>
        /// <param name="stringSelector">Selector to retrieve <see cref="string"/> from <typeparamref name="TAttribute"/>.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ConstraintBuilder<TState, string> AddRegexAttributeConstraint<TAttribute>(Func<TAttribute, string> stringSelector)
            where TAttribute : Attribute
        {
            return AddAttributeConstraint<TAttribute, string>((attribute, propertyInfo, ctx) =>
            {
                var pattern = stringSelector(attribute);
                var regex = ctx.GetOrSetState(pattern, p => new Regex(pattern, RegexOptions.Compiled));

                return new PatternConstraint<TState>(regex);
            });
        }

        /// <summary>
        /// Adds constraints when the property is annotated with <typeparamref name="TAttribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute.</typeparam>
        /// <typeparam name="TValue">Expected type of constraint.</typeparam>
        /// <returns>A <see cref="ConstraintBuilder{TValue}"/> instance.</returns>
        public ConstraintBuilder<TState, TValue> AddAttributeConstraint<TAttribute, TValue>(Func<TAttribute, PropertyInfo, BuilderContext<TState>, IConstraint<TState>> factory)
            where TAttribute : Attribute
        {
            return AddConstraint<TValue>((propertyInfo, ctx) =>
            {
                var attribute = propertyInfo.GetCustomAttribute<TAttribute>();

                if (attribute == null)
                {
                    return null;
                }

                return factory(attribute, propertyInfo, ctx);
            });
        }

        /// <summary>
        /// Adds a property filter for any property that has a return type that derives from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of property to find.</typeparam>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddPropertyFilter<T>()
        {
            return AddPropertyFilter(propertyInfo =>
            {
                return typeof(T).IsAssignableFrom(propertyInfo.PropertyType) || typeof(IEnumerable<T>).IsAssignableFrom(propertyInfo.PropertyType);
            });
        }

        /// <summary>
        /// Creates a property builder for <typeparamref name="TType"/>. If <typeparamref name="TType"/> is unknown,
        /// it will be added to the known types.
        /// </summary>
        /// <typeparam name="TType">Type to add property filters.</typeparam>
        /// <returns>A <see cref="ValidatorPropertyBuilder{TType, TState}"/> for <typeparamref name="TType"/>.</returns>
        public ValidatorPropertyBuilder<TType, TState> ForType<TType>() => new ValidatorPropertyBuilder<TType, TState>(this);

        /// <summary>
        /// Adds known types from an <see cref="Assembly"/> given a <paramref name="selector"/>.
        /// </summary>
        /// <param name="assembly">Assembly to search for exported types.</param>
        /// <param name="selector">Type selector.</param>
        /// <returns>The current <see cref="ValidatorBuilder{TState}"/>.</returns>
        public ValidatorBuilder<TState> AddKnownTypes(Assembly assembly, Func<Type, bool> selector)
        {
            var types = assembly.GetExportedTypes().Where(selector);

            return AddKnownTypes(types);
        }

        private ConstraintBuilder<TState, T> AddConstraint<T>(ConstraintBuilder<TState, T> builder)
        {
            AddKnownType(typeof(T));
            Constraints.Add(builder);
            return builder;
        }
    }
}
