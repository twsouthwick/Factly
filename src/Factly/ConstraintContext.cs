// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// Context object for <see cref="IConstraint{TState}"/>.
    /// </summary>
    /// <typeparam name="TState">Type of state object.</typeparam>
    public readonly struct ConstraintContext<TState>
    {
        private readonly ValidationContext<TState> _context;
        private readonly PropertyInfo _property;
        private readonly IConstraint<TState> _constraint;
        private readonly object _instance;
        private readonly object _value;

        internal ConstraintContext(
            ValidationContext<TState> context,
            IConstraint<TState> constraint,
            PropertyInfo property = null,
            object instance = null,
            object value = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _constraint = constraint ?? throw new ArgumentNullException(nameof(constraint));
            _property = property;
            _instance = instance;
            _value = value;
        }

        /// <summary>
        /// Gets the validation state.
        /// </summary>
        public TState State => _context == null ? default : _context.State;

        /// <summary>
        /// Raise an error for the current constraint and value.
        /// </summary>
        /// <param name="message">Message to raise as the error.</param>
        public void RaiseError(string message)
        {
            if (_context == null)
            {
                return;
            }

            _context.OnError(new ValidationError(_value, _instance, _property, _constraint.Id, _constraint.Context, message));
        }
    }
}
