// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace Factly
{
    [DebuggerDisplay("{Type.FullName,nq} {Property.Name,nq}")]
    [DebuggerTypeProxy(typeof(PropertyValidatorDebuggerProxy))]
    internal class PropertyValidator
    {
        private readonly IConstraint[] _constraints;

        private Func<object, object> _getter;

        private PropertyValidator(PropertyInfo property, bool shouldFollow, IConstraint[] constraints)
        {
            Property = property;
            _constraints = constraints;
            IncludeChildren = shouldFollow;
        }

        public PropertyInfo Property { get; }

        public Type Type => Property.PropertyType;

        public bool HasConstraints => _constraints.Length > 0;

        public bool IncludeChildren { get; }

        private Func<object, object> Getter
        {
            get
            {
                if (_getter == null)
                {
                    lock (this)
                    {
                        if (_getter == null)
                        {
                            _getter = Property.GetPropertyDelegate();
                        }
                    }
                }

                return _getter;
            }
        }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            if (!property.HasGetMethod())
            {
                return default;
            }

            var constraints = GetConstraints(property, builder.Constraints);
            var shouldFollow = ShouldFollow(property, builder.PropertyFilters);

            if (constraints.Length == 0 && !shouldFollow)
            {
                return default;
            }

            return new PropertyValidator(property, shouldFollow, constraints);
        }

        public object Validate(object item, ValidationContext context)
        {
            var value = Getter(item);

            foreach (var constraint in _constraints)
            {
                constraint.Validate(item, value, context);
            }

            return value;
        }

        private static bool ShouldFollow(PropertyInfo property, List<Func<PropertyInfo, bool>> selectors)
        {
            foreach (var selector in selectors)
            {
                if (selector(property))
                {
                    return true;
                }
            }

            return false;
        }

        private static IConstraint[] GetConstraints(PropertyInfo property, List<ConstraintBuilder> builders)
        {
            var array = default(ArrayBuilder<IConstraint>);

            foreach (var builder in builders)
            {
                var constraint = builder.Create(property);

                if (constraint != null)
                {
                    array.Add(constraint);
                }
            }

            return array.ToArray();
        }

#pragma warning disable CA1812
        internal class PropertyValidatorDebuggerProxy
#pragma warning restore CA1812
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
