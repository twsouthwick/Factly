// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace Factly
{
    /// <summary>
    /// Builder to generate a <see cref="Validator"/>.
    /// </summary>
    public sealed class ValidatorBuilder
    {
        private ValidatorBuilder()
        {
            Types = new HashSet<Type>();
            Constraints = new List<ConstraintBuilder>();
            PropertyFilters = new List<Func<PropertyInfo, bool>>();
            State = new StateManager();
        }

        internal StateManager State { get; }

        internal List<Func<PropertyInfo, bool>> PropertyFilters { get; }

        internal HashSet<Type> Types { get; }

        internal List<ConstraintBuilder> Constraints { get; }

        /// <summary>
        /// Creates an instance of <see cref="ValidatorBuilder"/>.
        /// </summary>
        /// <returns>A <see cref="ValidatorBuilder"/> to build a <see cref="Validator"/>.</returns>
        public static ValidatorBuilder Create() => new ValidatorBuilder();

        /// <summary>
        /// Adds a constraint generator that depends on an input <see cref="PropertyInfo"/>.
        /// </summary>
        /// <typeparam name="T">Type of the constraint.</typeparam>
        /// <param name="constraintGenerator">The generator to create a <see cref="IConstraint"/>.</param>
        /// <returns>A builder instance for the constraint.</returns>
        public ConstraintBuilder<T> AddConstraint<T>(Func<PropertyInfo, IConstraint> constraintGenerator)
        {
            var builder = new ConstraintBuilder<T>(constraintGenerator);
            Constraints.Add(builder);
            return builder;
        }

        /// <summary>
        /// Adds a constraint generator that depends on an input <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="constraintGenerator">The generator to create a <see cref="IConstraint"/>.</param>
        /// <returns>A builder instance for the constraint.</returns>
        public ConstraintBuilder AddConstraint(Func<PropertyInfo, IConstraint> constraintGenerator)
        {
            var builder = new ConstraintBuilder(constraintGenerator);
            Constraints.Add(builder);
            return builder;
        }

        /// <summary>
        /// Add a filter to identify properties that should be traversed.
        /// </summary>
        /// <param name="filter">A filter for a <see cref="PropertyInfo"/>.</param>
        /// <returns>The current <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder AddPropertyFilter(Func<PropertyInfo, bool> filter)
        {
            PropertyFilters.Add(filter);
            return this;
        }

        /// <summary>
        /// Add a collection of known types to the validator.
        /// </summary>
        /// <param name="types">Types to add.</param>
        /// <returns>The current <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder AddKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Types.Add(type);
            }

            return this;
        }

        /// <summary>
        /// Add a known type to the builder.
        /// </summary>
        /// <param name="type">Type to add.</param>
        /// <returns>The current <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder AddKnownType(Type type)
        {
            Types.Add(type);
            return this;
        }

#if !NO_CANCELLATION_TOKEN
        /// <summary>
        /// Build the <see cref="Validator"/> from the current information in <see cref="ValidatorBuilder"/>.
        /// </summary>
        /// <param name="token">Optional cancellation token.</param>
        /// <returns><see cref="Validator"/> built from <see cref="ValidatorBuilder"/>.</returns>
        public Validator Build(CancellationToken token = default)
        {
#else
        /// <summary>
        /// Build the <see cref="Validator"/> from the current information in <see cref="ValidatorBuilder"/>.
        /// </summary>
        /// <returns><see cref="Validator"/> built from <see cref="ValidatorBuilder"/>.</returns>
        public Validator Build()
        {
            var token = default(CancellationToken);
#endif
            if (Types.Count == 0)
            {
                throw new ValidatorBuilderException(SR.MustDeclareTypes, Errors.NoTypes, null, null);
            }

            var validators = new Dictionary<Type, TypeValidator>();
            var hasConstraints = false;

            IEnumerable<Type> AddItem(Type type)
            {
                var compiledType = new TypeValidator(type, this);
                validators.Add(type, compiledType);

                foreach (var property in compiledType.Properties)
                {
                    if (property.HasConstraints)
                    {
                        hasConstraints = true;
                    }

                    if (property.IncludeChildren)
                    {
                        yield return property.Type;
                    }
                }
            }

            Types.Traverse(AddItem, token);

            if (!hasConstraints)
            {
                throw new ValidatorBuilderException(SR.NoConstraints, Errors.NoConstraintsFound, null, null);
            }

            return new Validator(validators);
        }
    }
}
