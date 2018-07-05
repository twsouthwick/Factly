// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ObjectValidator
{
    [DebuggerDisplay("{Type.FullName,nq} {Property.Name,nq}")]
    [DebuggerTypeProxy(typeof(PropertyValidatorDebuggerProxy))]
    internal readonly struct PropertyValidator
    {
        private readonly Func<object, object> _getter;
        private readonly IConstraint[] _constraints;

        private PropertyValidator(PropertyInfo property, bool shouldFollow, IConstraint[] constraints)
        {
            Property = property;
            _getter = property.GetPropertyDelegate();
            _constraints = constraints;
            ShouldFollow = shouldFollow;
        }

        public PropertyInfo Property { get; }

        public Type Type => Property.PropertyType;

        public bool ShouldFollow { get; }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            var constraints = builder.Constraints
                .Select(factory => factory(property))
                .Where(constraint => constraint != null)
                .ToArray();
            var shouldFollow = builder.PropertyFilters.Any(t => t(property));

            if (constraints.Length == 0 && !shouldFollow)
            {
                return default;
            }

            return new PropertyValidator(property, shouldFollow, constraints);
        }

        public object Validate(object item, ValidationContext context)
        {
            var value = _getter(item);

            foreach (var constraint in _constraints)
            {
                constraint.Validate(item, value, context);
            }

            return value;
        }

        internal class PropertyValidatorDebuggerProxy
        {
            public PropertyValidatorDebuggerProxy(PropertyValidator validator)
            {
                Constraints = validator._constraints;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IConstraint[] Constraints { get; }
        }
    }
}
