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
    /// <typeparam name="TState">Custom type supplied for the validation.</typeparam>
    /// <typeparam name="TValue">Type to build up constraints.</typeparam>
    public class ConstraintBuilder<TState, TValue> : ConstraintBuilder<TState>
    {
        private readonly Dictionary<Type, Func<object, TValue>> _mappers = new Dictionary<Type, Func<object, TValue>>();

        internal ConstraintBuilder(ValidatorBuilder<TState> builder, Func<PropertyInfo, BuilderContext<TState>, IConstraint<TState>> factory)
            : base(factory)
        {
            ExpandEnumerables(builder);
        }

        internal ConstraintBuilder(ValidatorBuilder<TState> builder, Func<Type, BuilderContext<TState>, IConstraint<TState>> factory)
            : base(factory)
        {
            ExpandEnumerables(builder);
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

        internal override IConstraint<TState> Create(PropertyInfo property, BuilderContext<TState> context)
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
            else if (typeof(IEnumerable<TValue>).IsAssignableFrom(property.PropertyType))
            {
                return new EnumerableConstraint(constraint);
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

        private static void ExpandEnumerables(ValidatorBuilder<TState> builder)
        {
            builder.AddConstraint((property, ctx) =>
            {
                if (typeof(IEnumerable<TValue>).IsAssignableFrom(property.PropertyType))
                {
                    return ExpansionConstraint.Instance;
                }

                return null;
            });
        }

        private class EnumerableConstraint : IConstraint<TState>, IConstraintEnumerable
        {
            private readonly IConstraint<TState> _other;

            public EnumerableConstraint(IConstraint<TState> other)
            {
                _other = other;
            }

            public string Id => _other.Id;

            public object Context => _other.Context;

            public IEnumerable<object> GetItems(object instance) => (IEnumerable<object>)instance;

            public void Validate(object value, ConstraintContext<TState> context) => _other.Validate(value, context);
        }

        private class TypedConstraint : IConstraint<TState>, IObjectConverter
        {
            private readonly IConstraint<TState> _constraint;
            private readonly Func<object, TValue> _func;

            public TypedConstraint(IConstraint<TState> constraint, Func<object, TValue> func)
            {
                _constraint = constraint;
                _func = func;
            }

            public string Id => _constraint.Id;

            public object Context => _constraint.Context;

            public object Convert(object value) => _func(value);

            public void Validate(object value, ConstraintContext<TState> context) => _constraint.Validate(value, context);
        }

        private class ExpansionConstraint : IConstraint<TState>, IConstraintEnumerable
        {
            private ExpansionConstraint()
            {
            }

            public static ExpansionConstraint Instance { get; } = new ExpansionConstraint();

            public string Id => nameof(ExpansionConstraint);

            public object Context => null;

            public IEnumerable<object> GetItems(object instance) => (IEnumerable<object>)instance;

            public void Validate(object value, ConstraintContext<TState> context)
            {
            }
        }
    }
}
