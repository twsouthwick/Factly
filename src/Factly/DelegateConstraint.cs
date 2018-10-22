// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    internal class DelegateConstraint<T> : IConstraint
    {
        private readonly Func<T, bool> _constraint;

        public DelegateConstraint(Func<T, bool> constraint)
        {
            _constraint = constraint;
        }

        public string Id => "DelegateConstraint";

        public object Context => null;

        public bool Validate(object value) => _constraint((T)value);
    }
}
