// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Factly
{
    internal class ExpansionConstraint<TState> : IConstraint<TState>, IConstraintEnumerable
    {
        private ExpansionConstraint()
        {
        }

        public static ExpansionConstraint<TState> Instance { get; } = new ExpansionConstraint<TState>();

        public string Id => nameof(ExpansionConstraint<TState>);

        public object Context => null;

        public IEnumerable<object> GetItems(object instance) => (IEnumerable<object>)instance;

        public void Validate(object value, ConstraintContext<TState> context)
        {
        }
    }
}
