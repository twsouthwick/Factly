// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ObjectValidator
{
    internal class PatternConstraint : IConstraint
    {
        private readonly Regex _regex;
        private readonly PropertyInfo _property;

        public PatternConstraint(PropertyInfo property, string pattern)
        {
            if (pattern == null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            if (property.PropertyType != typeof(string))
            {
                throw new ValidatorException(SR.PatternRequiresStringProperty, Errors.PatternAppliedToNonString, property.DeclaringType, property);
            }

            _regex = new Regex(pattern, RegexOptions.Compiled);
            _property = property;
        }

        public void Validate(object instance, object value, ValidationContext context)
        {
            if (value == null)
            {
                context.OnError(new PatternValidationError(instance, _property, _regex, value));
                return;
            }

            Debug.Assert(value is string, "This is checked in the constructor so all values supplied here should be a string");

            if (!_regex.IsMatch((string)value))
            {
                context.OnError(new PatternValidationError(instance, _property, _regex, value));
            }
        }
    }
}
