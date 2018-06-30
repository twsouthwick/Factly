using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    internal class ValidationErrorFilterObserver : ValidationObserver
    {
        private readonly ValidationContext _context;
        private readonly ValidatorBuilder _builder;
        private readonly HashSet<object> _visited;

        public ValidationErrorFilterObserver(ValidationContext context, in ValidatorBuilder builder)
        {
            _context = context;
            _builder = builder;
            _visited = new HashSet<object>();
        }

        public ValidationResult Result { get; } = new ValidationResult();

        public void Validate(object instance)
        {
            if (!_visited.Add(instance))
            {
                return;
            }

            Result.ObjectsTested++;

            var recurse = new List<object>();

            foreach (var property in instance.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(instance);

                if (_builder.Pattern is PatternConstraint pattern)
                {
                    var regex = pattern.GetRegex(property);

                    InvalidPattern(instance, regex, (string)propertyValue);
                }

                if (propertyValue != null && _builder.RecursiveHandler?.Invoke(property) == true)
                {
                    recurse.Add(propertyValue);
                }
            }

            foreach (var item in recurse)
            {
                Validate(item);
            }
        }

        public override void InvalidPattern(object instance, Regex pattern, string value)
        {
            if (pattern == null)
            {
                return;
            }

            if (value == null || !pattern.IsMatch(value))
            {
                Result.Increment();
                _context.Observer.InvalidPattern(instance, pattern, value);
            }
        }
    }
}
