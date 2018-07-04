// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text.RegularExpressions;

namespace ObjectValidator
{
    /// <summary>
    /// An error that occurs when a pattern is encountered that doesn't match
    /// </summary>
    public class PatternValidationError : ValidationError
    {
        internal PatternValidationError(object instance, PropertyInfo property, Regex pattern, object value)
            : base(instance, property)
        {
            Pattern = pattern;
            Value = value;
        }

        /// <summary>
        /// Gets the value of the instance
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Gets the expected pattern
        /// </summary>
        public Regex Pattern { get; }
    }
}
