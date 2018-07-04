// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ObjectValidator
{
    internal class DelegateConstraint : IConstraint
    {
        private readonly Action<object, object, ValidationContext> _action;

        public DelegateConstraint(Action action)
            : this((instance, value, ctx) => action())
        {
        }

        public DelegateConstraint(Action<object, object, ValidationContext> func)
        {
            _action = func;
        }

        public void Validate(object instance, object value, ValidationContext context) => _action(instance, value, context);
    }
}
