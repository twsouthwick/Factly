// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace Factly
{
    /// <summary>
    /// Extensions to add well known DataAnnotation constraints.
    /// </summary>
    public static class DataAnnotationExtensions
    {
        /// <summary>
        /// Add a regular expression constraint using <see cref="RegularExpressionAttribute"/>.
        /// </summary>
        /// <param name="builder">The current <see cref="ValidatorBuilder{TState}"/>.</param>
        /// <typeparam name="TState">Custom type supplied for the validation.</typeparam>
        /// <returns>A constraint builder instance.</returns>
        public static ConstraintBuilder<TState, string> AddRegexAttributeConstraint<TState>(this ValidatorBuilder<TState> builder)
        {
            return builder
                .AddRegexAttributeConstraint<RegularExpressionAttribute>(pattern => pattern.Pattern);
        }
    }
}
