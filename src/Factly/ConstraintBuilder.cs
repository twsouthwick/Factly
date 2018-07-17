﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reflection;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace Factly
{
    /// <summary>
    /// A non generic constraint.
    /// </summary>
    public class ConstraintBuilder : IConstraintBuilder
    {
        private readonly Func<PropertyInfo, IConstraint> _factory;

        internal ConstraintBuilder(Func<PropertyInfo, IConstraint> factory)
        {
            _factory = factory;
        }

        IConstraint IConstraintBuilder.Create(PropertyInfo property) => CreateInternal(property);

        private protected virtual IConstraint CreateInternal(PropertyInfo property) => _factory(property);
    }
}
