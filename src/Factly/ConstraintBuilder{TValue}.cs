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
        private readonly Dictionary<Type, Func<object, TValue>> _mappers;

        internal ConstraintBuilder(Func<PropertyInfo, IConstraint> factory)
            : base(factory)
        {
            _mappers = new Dictionary<Type, Func<object, TValue>>();
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

        internal override IConstraint Create(PropertyInfo property)
        {
            if (property.PropertyType == typeof(TValue))
            {
                return base.Create(property);
            }
            else if (_mappers.TryGetValue(property.PropertyType, out var func))
            {
                return new TypedConstraint(base.Create(property), property, func);
            }
            else
            {
                throw new ValidatorException(Errors.PatternAppliedToNonString, Errors.PatternAppliedToNonString, property.DeclaringType, property);
            }
        }

        private class TypedConstraint : IConstraint
        {
            private readonly IConstraint _constraint;
            private readonly PropertyInfo _property;
            private readonly Func<object, TValue> _func;

            public TypedConstraint(IConstraint constraint, PropertyInfo property, Func<object, TValue> func)
            {
                _constraint = constraint;
                _property = property;
                _func = func;
            }

            public void Validate(object instance, object value, ValidationContext context)
            {
                _constraint.Validate(instance, _func(value), context);
            }
        }
    }
}
