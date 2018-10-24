// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    /// <summary>
    /// Defines a property constraint for use in validating.
    /// </summary>
    /// <typeparam name="TState">Custom type supplied for the validation.</typeparam>
    public interface IConstraint<TState>
    {
        /// <summary>
        /// Gets the ID of the constraint.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets any constraint specific context.
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Called to validate a property value.
        /// </summary>
        /// <param name="value">The value of the property.</param>
        /// <param name="context">Validation context.</param>
        /// <returns>Whether validation was successful or not.</returns>
        bool Validate(object value, TState context);
    }
}
