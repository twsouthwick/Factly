// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    /// <summary>
    /// An implementation of <see cref="IConstraint{TState}"/> that wraps a delegate.
    /// </summary>
    /// <typeparam name="TState">Custom type supplied for the validation.</typeparam>
    /// <typeparam name="T">The expected type of a constraint value.</typeparam>
    public class DelegateConstraint<TState, T> : IConstraint<TState>
    {
        private readonly Action<T, ValidationContext<TState>> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{TState, T}"/> class.
        /// </summary>
        /// <param name="constraint">The constraint delegate.</param>
        /// <param name="id">The id of the constraint.</param>
        /// <param name="context">An option context value.</param>
        public DelegateConstraint(Action<T, ValidationContext<TState>> constraint, string id, object context = null)
        {
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Context = context;
        }

        /// <inheritdoc/>
        public string Id { get; }

        /// <inheritdoc/>
        public object Context { get; }

        /// <inheritdoc/>
        public void Validate(object value, ValidationContext<TState> context) => _constraint((T)value, context);
    }
}
