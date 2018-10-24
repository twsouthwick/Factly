// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// A non generic constraint.
    /// </summary>
    public class ConstraintBuilder<TState>
    {
        private readonly Func<PropertyInfo, BuilderContext<TState>, IConstraint<TState>> _propertyConstraintfactory;
        private readonly Func<Type, BuilderContext<TState>, IConstraint<TState>> _typeConstraintFactory;

        internal ConstraintBuilder(Func<PropertyInfo, BuilderContext<TState>, IConstraint<TState>> factory)
        {
            _propertyConstraintfactory = factory;
        }

        internal ConstraintBuilder(Func<Type, BuilderContext<TState>, IConstraint<TState>> factory)
        {
            _typeConstraintFactory = factory;
        }

        internal virtual IConstraint<TState> Create(PropertyInfo property, BuilderContext<TState> context) => _propertyConstraintfactory?.Invoke(property, context);

        internal virtual IConstraint<TState> Create(Type type, BuilderContext<TState> context) => _typeConstraintFactory?.Invoke(type, context);
    }
}
