// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Factly
{
    /// <summary>
    /// A collection of extension methods for helper functions for <see cref="ValidatorBuilder{TOptions}"/>.
    /// </summary>
    public static class ValidatorBuilderExtensions
    {
        /// <summary>
        /// Adds a constraint generator that depends on a <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="builder">Current <see cref="ValidatorBuilder{TOptions}"/>.</param>
        /// <param name="factory">The factory to create a constraint given an attribute instance.</param>
        /// <returns>A <see cref="ConstraintBuilder{TValue}"/> instance.</returns>
        public static ConstraintBuilder<TState> AddConstraint<TState>(this ValidatorBuilder<TState> builder, Func<PropertyInfo, IConstraint<TState>> factory)
        {
            return builder.AddConstraint<TState>((property, _) => factory(property));
        }
    }
}
