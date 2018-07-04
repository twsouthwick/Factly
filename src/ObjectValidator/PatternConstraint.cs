﻿using System;
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
                throw new ValidatorException("Type of property must be string if a pattern is specified", Errors.PatternAppliedToNonString, property.DeclaringType, property);
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

            Debug.Assert(value is string);

            if (!_regex.IsMatch((string)value))
            {
                context.OnError(new PatternValidationError(instance, _property, _regex, value));
            }
        }
    }
}
