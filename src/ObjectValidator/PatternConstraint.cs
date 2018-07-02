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
                throw new ValidatorException("Type of property must be string if a pattern is specified", StructuralErrors.PatternAppliedToNonString, property.DeclaringType, property);
            }

            _regex = new Regex(pattern, RegexOptions.Compiled);
            _property = property;
        }

        public ValidationError Validate(object instance, object value)
        {
            if (value == null)
            {
                return new PatternValidationError(instance, _property, _regex, value);
            }

            Debug.Assert(value is string);

            if (_regex.IsMatch((string)value))
            {
                return null;
            }

            return new PatternValidationError(instance, _property, _regex, value);
        }
    }
}
