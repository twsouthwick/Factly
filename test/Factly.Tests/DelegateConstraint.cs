// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    internal class DelegateConstraint : IConstraint
    {
        private readonly Func<object, bool> _func;

        public DelegateConstraint(Func<bool> func)
            : this(_ => func())
        {
        }

        public DelegateConstraint(Func<object, bool> func)
        {
            _func = func;
        }

        public string Id => nameof(DelegateConstraint);

        public object Context => null;

        public bool Validate(object value) => _func(value);
    }
}
