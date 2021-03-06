﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    internal class DelegateConstraint : DelegateConstraint<object>
    {
        public DelegateConstraint(Func<bool> func)
            : base(_ => func())
        {
        }

        public DelegateConstraint(Func<object, bool> func)
            : base((obj, _) => func(obj))
        {
        }
    }
}
