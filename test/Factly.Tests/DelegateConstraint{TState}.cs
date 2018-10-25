// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    internal class DelegateConstraint<TState> : DelegateConstraint<TState, object>
    {
        public DelegateConstraint(Action func)
            : this(_ => func())
        {
        }

        public DelegateConstraint(Action<object> func)
            : this((obj, _) => func(obj))
        {
        }

        public DelegateConstraint(Action<object, ValidationContext<TState>> func)
            : base(func, Guid.NewGuid().ToString())
        {
        }
    }
}
