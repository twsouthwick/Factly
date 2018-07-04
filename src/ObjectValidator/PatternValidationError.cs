// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using System.Text.RegularExpressions;

namespace ObjectValidator
{
    public class PatternValidationError : ValidationError
    {
        internal PatternValidationError(object instance, PropertyInfo property, Regex pattern, object value)
            : base(instance, property)
        {
            Pattern = pattern;
            Value = value;
        }

        public object Value { get; }

        public Regex Pattern { get; }
    }
}
