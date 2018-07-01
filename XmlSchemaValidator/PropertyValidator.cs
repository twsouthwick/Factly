using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    internal readonly struct PropertyValidator
    {
        private readonly PropertyInfo _property;
        private readonly Func<object, object> _getter;
        private readonly Regex _pattern;

        private PropertyValidator(PropertyInfo property, bool shouldDescend, Regex pattern)
        {
            _property = property;
            _getter = property.GetValue;
            _pattern = pattern;
            Type = property.PropertyType;
            ShouldDescend = shouldDescend;
        }

        public void Validate(object item, ValidationProcessor processor)
        {
            var value = _getter(item);

            if (_pattern != null)
            {
                var strValue = (string)value;
                if (strValue == null || !_pattern.IsMatch(strValue))
                {
                    processor.Context.PatternErrors?.OnNext(new PatternValidationError(item, _property, _pattern, strValue));
                }
            }

            if (ShouldDescend)
            {
                processor.Validate(value);
            }
        }

        public Type Type { get; }

        public bool ShouldDescend { get; }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            var pattern = builder.Pattern?.GetRegex(property);
            var shouldDescend = builder.IsDescendant?.Invoke(property) ?? false;

            if (pattern != null && property.PropertyType != typeof(string))
            {
                throw new ValidatorException("Type of property must be string if a pattern is specified", StructuralErrors.PatternAppliedToNonString, property.DeclaringType, property);
            }

            if (pattern == null && !shouldDescend)
            {
                return default;
            }

            return new PropertyValidator(property, shouldDescend, pattern);
        }
    }
}
