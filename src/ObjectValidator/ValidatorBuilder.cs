// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectValidator
{
    public sealed class ValidatorBuilder
    {
        private ValidatorBuilder()
        {
            Types = new HashSet<Type>();
            Constraints = new List<Func<PropertyInfo, IConstraint>>();
            PropertyFilters = new List<Func<PropertyInfo, bool>>();
        }

        internal PatternConstraint Pattern { get; private set; }

        internal List<Func<PropertyInfo, bool>> PropertyFilters { get; }

        internal HashSet<Type> Types { get; }

        internal List<Func<PropertyInfo, IConstraint>> Constraints { get; }

        public static ValidatorBuilder Create() => new ValidatorBuilder();

        public ValidatorBuilder AddConstraint(Func<PropertyInfo, IConstraint> constraint)
        {
            Constraints.Add(constraint);
            return this;
        }

        public ValidatorBuilder AddPropertyFilter(Func<PropertyInfo, bool> filter)
        {
            PropertyFilters.Add(filter);
            return this;
        }

        public ValidatorBuilder AddKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Types.Add(type);
            }

            return this;
        }

        public ValidatorBuilder AddKnownType(Type type)
        {
            Types.Add(type);
            return this;
        }

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
                        if (property.ShouldFollow)
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
