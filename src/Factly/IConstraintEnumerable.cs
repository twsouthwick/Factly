// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Factly
{
    /// <summary>
    /// An interface that allows extracting a series of values from a given instance.
    /// </summary>
    internal interface IConstraintEnumerable
    {
        /// <summary>
        /// Gets all the items from a supplied instance.
        /// </summary>
        /// <param name="instance">Instance to expand.</param>
        /// <returns>A series of items.</returns>
        IEnumerable<object> GetItems(object instance);
    }
}
