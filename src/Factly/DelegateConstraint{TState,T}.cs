﻿// Copyright (c) Taylor Southwick. All rights reserved.
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
        private readonly Func<T, TState, bool> _constraint;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateConstraint{TState, T}"/> class.
        /// </summary>
        /// <param name="constraint">The constraint delegate.</param>
        /// <param name="id">The id of the constraint.</param>
        /// <param name="context">An option context value.</param>
        public DelegateConstraint(Func<T, TState, bool> constraint, string id, object context = null)
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
        public bool Validate(object value, TState context) => _constraint((T)value, context);
    }
}