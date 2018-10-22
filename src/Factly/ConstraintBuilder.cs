// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// A non generic constraint.
    /// </summary>
    public class ConstraintBuilder
    {
        private readonly Func<PropertyInfo, BuilderContext, IConstraint> _propertyConstraintfactory;
        private readonly Func<Type, BuilderContext, IConstraint> _typeConstraintFactory;

        internal ConstraintBuilder(Func<PropertyInfo, BuilderContext, IConstraint> factory)
        {
            _propertyConstraintfactory = factory;
        }

        internal ConstraintBuilder(Func<Type, BuilderContext, IConstraint> factory)
        {
            _typeConstraintFactory = factory;
        }

        internal virtual IConstraint Create(PropertyInfo property, BuilderContext context) => _propertyConstraintfactory?.Invoke(property, context);

        internal virtual IConstraint Create(Type type, BuilderContext context) => _typeConstraintFactory?.Invoke(type, context);
    }
}
