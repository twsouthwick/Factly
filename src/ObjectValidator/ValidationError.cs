// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace ObjectValidator
{
    /// <summary>
    /// A class that contains basic validation error information
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="instance">The instance that raised the error</param>
        /// <param name="property">The property that raised the error</param>
        public ValidationError(object instance, PropertyInfo property)
        {
            Instance = instance;
            Property = property;
        }

        /// <summary>
        /// Gets the instance that raised the error
        /// </summary>
        public object Instance { get; }

        /// <summary>
        /// Gets the property that raised the error
        /// </summary>
        public PropertyInfo Property { get; }
    }
}
