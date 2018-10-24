// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// A constraint builder for <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">Type to build up constraints.</typeparam>
    public class ConstraintBuilder<TValue> : ConstraintBuilder
    {
        private readonly Dictionary<Type, Func<object, TValue>> _mappers = new Dictionary<Type, Func<object, TValue>>();

        internal ConstraintBuilder(Func<PropertyInfo, BuilderContext, IConstraint> factory)
            : base(factory)
        {
        }

        internal ConstraintBuilder(Func<Type, BuilderContext, IConstraint> factory)
            : base(factory)
        {
        }

        /// <summary>
        /// Add a mapping function between <typeparamref name="TFrom"/> and <typeparamref name="TValue"/>.
        /// </summary>
        /// <typeparam name="TFrom">The type to convert from.</typeparam>
        /// <param name="mapper">The mapping function.</param>
        public void AddTypeMapper<TFrom>(Func<TFrom, TValue> mapper)
        {
            _mappers.Add(typeof(TFrom), obj =>
            {
                if (obj is null)
                {
                    return mapper(default);
                }
                else
                {
                    return mapper((TFrom)obj);
                }
            });
        }

        internal override IConstraint Create(PropertyInfo property, BuilderContext context)
        {
            var constraint = base.Create(property, context);

            if (constraint is null)
            {
                return null;
            }
            else if (property.PropertyType == typeof(TValue))
            {
                return constraint;
            }
            else if (_mappers.TryGetValue(property.PropertyType, out var func))
            {
                return new TypedConstraint(constraint, func);
            }
            else
            {
                throw new ValidatorBuilderException(SR.UnknownTypeEncountered, Errors.UnsupportedTypeForConstraint, property.DeclaringType, property);
            }
        }

        private class TypedConstraint : IConstraint, IObjectConverter
        {
            private readonly IConstraint _constraint;
            private readonly Func<object, TValue> _func;

            public TypedConstraint(IConstraint constraint, Func<object, TValue> func)
            {
                _constraint = constraint;
                _func = func;
            }

            public string Id => _constraint.Id;

            public object Context => _constraint.Context;

            public object Convert(object value) => _func(value);

            public bool Validate(object value) => _constraint.Validate(value);
        }
    }
}
