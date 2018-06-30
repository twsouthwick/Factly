using System.Collections.Generic;
using System.Reflection;

namespace XmlSchemaValidator
{
    internal readonly struct ValidationProcessor
    {
        private readonly ValidationContext _context;
        private readonly ValidatorBuilder _builder;
        private readonly HashSet<object> _visited;

        public ValidationProcessor(ValidationContext context, in ValidatorBuilder builder)
        {
            _builder = builder;
            _visited = new HashSet<object>();
            _context = context ?? new DefaultValidationContext();
        }

        public void Validate(object instance)
        {
            if (!_visited.Add(instance))
            {
                return;
            }

            _context.Items?.OnNext(instance);

            var childrenList = new DescendantList<object>();

            foreach (var property in instance.GetType().GetProperties())
            {
                var propertyValue = property.GetValue(instance);

                if (_builder.Pattern is PatternConstraint pattern)
                {
                    ValidatePattern(pattern, instance, property, propertyValue);
                }

                if (propertyValue != null && _builder.IsDescendant?.Invoke(property) == true)
                {
                    childrenList.Add(propertyValue);
                }
            }

            foreach (var item in childrenList)
            {
                Validate(item);
            }
        }

        private void ValidatePattern(PatternConstraint constraint, object instance, PropertyInfo property, object propertyValue)
        {
            if (property.PropertyType != typeof(string))
            {
                _context.StructuralErrors?.OnNext(new StructuralError(StructuralErrors.PatternAppliedToNonString, instance, property));
                return;
            }

            var pattern = constraint.GetRegex(property);

            if (pattern == null)
            {
                return;
            }

            var value = (string)propertyValue;

            if (value == null || !pattern.IsMatch(value))
            {
                _context.PatternErrors?.OnNext(new PatternValidationError(instance, property, pattern, propertyValue));
            }
        }
    }
}
