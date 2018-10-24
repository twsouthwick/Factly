// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Factly
{
    /// <summary>
    /// A class that contains basic validation error information.
    /// </summary>
    public sealed class ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="value">The value that raised the error.</param>
        /// <param name="instance">The instance that contains the value.</param>
        /// <param name="property">The property that raised the error.</param>
        /// <param name="id">Id of constraint that raised the error.</param>
        /// <param name="context">Context contained in the constraint of the raised the error.</param>
        internal ValidationError(
            object value,
            object instance,
            PropertyInfo property,
            string id,
            object context)
        {
            Value = value;
            Instance = instance;
            Property = property;
            Id = id;
            Context = context;
        }

        /// <summary>
        /// Gets the ID of the constraint that raised the error.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the context of the constraint.
        /// </summary>
        public object Context { get; }

        /// <summary>
        /// Gets the value that raised the error.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the instance that raised the error.
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Gets the property that raised the error.
        /// </summary>
        public PropertyInfo Property { get; }
    }
}
