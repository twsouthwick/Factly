// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Factly
{
    internal interface IConstraintBuilder
    {
        IConstraint Create(PropertyInfo property);
    }
}
