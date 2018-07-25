// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    /// <summary>
    /// An abstraction of an <see cref="IConstraint"/> that can convert values.
    /// </summary>
    internal interface ITypedConstraint : IConstraint
    {
        /// <summary>
        /// Converts values to an expected type.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>The converted value.</returns>
        object Convert(object value);
    }
}
