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
    internal class PropertyValidator<TState>
    {
        private readonly ReadonlyArray<IConstraint<TState>> _constraints;

        private Func<object, object> _getter;

        private PropertyValidator(PropertyInfo property, bool shouldFollow, ReadonlyArray<IConstraint<TState>> constraints)
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

        public static PropertyValidator<TState> Create(PropertyInfo property, BuilderContext<TState> context)
        {
            if (!property.HasGetMethod())
            {
                return default;
            }

            var constraints = GetConstraints(property, context.Builder.Constraints, context);
            var shouldFollow = ShouldFollow(property, context.Builder.PropertyFilters);

            if (constraints.Length == 0 && !shouldFollow)
            {
                return default;
            }

            return new PropertyValidator<TState>(property, shouldFollow, constraints);
        }

        public object Validate(object item, ValidationContext<TState> context)
        {
            var value = Getter(item);

            foreach (var constraint in _constraints)
            {
                var updated = constraint is IObjectConverter converter ? converter.Convert(value) : value;

                constraint.Validate(updated, context.Clone(Property, constraint, item));
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

        private static ReadonlyArray<IConstraint<TState>> GetConstraints(PropertyInfo property, List<ConstraintBuilder<TState>> builders, BuilderContext<TState> context)
        {
            var array = new ArrayBuilder<IConstraint<TState>>(builders.Count);

            foreach (var builder in builders)
            {
                var constraint = builder.Create(property, context);

                if (constraint != null)
                {
                    array.Add(constraint);
                }
            }

            return array.Build();
        }
    }
}
