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
        private readonly Func<PropertyInfo, BuilderContext, IConstraint> _factory;

        internal ConstraintBuilder(Func<PropertyInfo, BuilderContext, IConstraint> factory)
        {
            _factory = factory;
        }

        internal virtual IConstraint Create(PropertyInfo property, BuilderContext context) => _factory(property, context);
    }
}
