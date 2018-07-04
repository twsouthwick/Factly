// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;

namespace ObjectValidator
{
    public static class DataAnnotationExtensions
    {
        public static ValidatorBuilder AddRegexConstraint(this ValidatorBuilder builder)
        {
            return builder
                .AddRegexConstraint<RegularExpressionAttribute>(pattern => pattern.Pattern);
        }
    }
}
