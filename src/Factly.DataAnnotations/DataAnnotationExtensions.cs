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
        /// <param name="builder">The current <see cref="ValidatorBuilder"/>.</param>
        /// <returns><paramref name="builder"/>.</returns>
        public static ValidatorBuilder AddRegexConstraint(this ValidatorBuilder builder)
        {
            return builder
                .AddRegexConstraint<RegularExpressionAttribute>(pattern => pattern.Pattern);
        }
    }
}
