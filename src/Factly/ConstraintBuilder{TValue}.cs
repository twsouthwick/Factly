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

    public class ConstraintBuilder<TValue> : ConstraintBuilder
    {
        private readonly Dictionary<Type, Func<object, TValue>> _mappers;

        internal ConstraintBuilder(Func<PropertyInfo, IConstraint> factory)
            : base(factory)
        {
            _mappers = new Dictionary<Type, Func<object, TValue>>();
        }

        public void AddTypeMapper<T>(Func<T, TValue> mapper)
        {
            _mappers.Add(typeof(T), obj => mapper((T)obj));
        }

        private protected override IConstraint CreateInternal(PropertyInfo property)
        {
            if (property.PropertyType == typeof(TValue))
            {
                return base.CreateInternal(property);
            }
            else
            {
                return new ProxyConstraint(base.CreateInternal(property), property, _mappers);
            }
        }

        private class ProxyConstraint : IConstraint
        {
            private readonly IConstraint _constraint;
            private readonly PropertyInfo _property;
            private readonly Dictionary<Type, Func<object, TValue>> _mappers;

            public ProxyConstraint(IConstraint constraint, PropertyInfo property, Dictionary<Type, Func<object, TValue>> mappers)
            {
                _constraint = constraint;
                _property = property;
                _mappers = mappers;
            }

            public void Validate(object instance, object value, ValidationContext context)
            {
                if (value is TValue)
                {
                    _constraint.Validate(instance, value, context);
                }
                else if (_mappers.TryGetValue(value.GetType(), out var map))
                {
                    _constraint.Validate(instance, map(value), context);
                }

                context.OnError(new ValidationError(instance, _property));
            }
        }
    }
}
