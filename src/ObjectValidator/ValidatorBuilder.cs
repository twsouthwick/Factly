// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectValidator
{
    /// <summary>
    /// Builder to generate a <see cref="Validator"/>.
    /// </summary>
    public sealed class ValidatorBuilder
    {
        private ValidatorBuilder()
        {
            Types = new HashSet<Type>();
            Constraints = new List<Func<PropertyInfo, IConstraint>>();
            PropertyFilters = new List<Func<PropertyInfo, bool>>();
            State = new StateManager();
        }

        internal StateManager State { get; }

        internal List<Func<PropertyInfo, bool>> PropertyFilters { get; }

        internal HashSet<Type> Types { get; }

        internal List<Func<PropertyInfo, IConstraint>> Constraints { get; }

        /// <summary>
        /// Creates an instance of <see cref="ValidatorBuilder"/>.
        /// </summary>
        /// <returns>A <see cref="ValidatorBuilder"/> to build a <see cref="Validator"/>.</returns>
        public static ValidatorBuilder Create() => new ValidatorBuilder();

        /// <summary>
        /// Adds a constraint generator that depends on an input <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="constraintGenerator">The generator to create a <see cref="IConstraint"/>.</param>
        /// <returns>The current <see cref="ValidatorBuilder"/>.</returns>
        public ValidatorBuilder AddConstraint(Func<PropertyInfo, IConstraint> constraintGenerator)
        {
            Constraints.Add(constraintGenerator);
            return this;
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

        /// <summary>
        /// Build the <see cref="Validator"/> from the current information in <see cref="ValidatorBuilder"/>.
        /// </summary>
        /// <returns><see cref="Validator"/> built from <see cref="ValidatorBuilder"/>.</returns>
        public Validator Build()
        {
            if (Types.Count == 0)
            {
                throw new ValidatorException(SR.MustDeclareTypes, Errors.NoTypes, null, null);
            }

            var visited = new HashSet<Type>();
            var left = new Queue<Type>(Types);
            var validators = new Dictionary<Type, TypeValidator>();

            while (left.Count > 0)
            {
                var type = left.Dequeue();
                if (visited.Add(type))
                {
                    var compiledType = new TypeValidator(type, this);
                    validators.Add(type, compiledType);

                    foreach (var property in compiledType.Properties)
                    {
                        if (property.IncludeChildren)
                        {
                            left.Enqueue(property.Type);
                        }
                    }
                }
            }

            return new Validator(validators);
        }
    }
}
