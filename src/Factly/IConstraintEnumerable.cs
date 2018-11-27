// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Factly
{
    internal interface IConstraintEnumerable
    {
        IEnumerable<object> GetItems(object instance);
    }
}
