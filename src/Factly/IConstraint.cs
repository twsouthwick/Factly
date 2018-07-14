// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    /// <summary>
    /// Defines a property constraint for use in validating.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// Called to validate a property value.
        /// </summary>
        /// <param name="instance">Instance the property is on.</param>
        /// <param name="value">The value of the property.</param>
        /// <param name="context">A <see cref="ValidationContext"/> instance.</param>
        void Validate(object instance, object value, ValidationContext context);
    }
}
