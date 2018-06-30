using System.Collections.Generic;
using System.Reflection;

namespace XmlSchemaValidator
{
    internal partial struct ValidationVisitor
    {
        private readonly ValidationContext _context;
        private readonly ValidatorBuilder _builder;
        private readonly HashSet<object> _visited;
        private readonly ValidationResult _result;

        public ValidationVisitor(ValidationContext context, in ValidatorBuilder builder)
        {
            _context = context;
            _builder = builder;
            _visited = new HashSet<object>();
            _result = new ValidationResult();
        }

        public ValidationResult Result
        {
            get
            {
                _result.ObjectsTested = _visited.Count;
                return _result;
            }
        }

        public void Validate(object instance)
        {
            if (!_visited.Add(instance))
            {
                return;
            }

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
            var pattern = constraint.GetRegex(property);

            if (pattern == null)
            {
                return;
            }

            var value = (string)propertyValue;

            if (value == null || !pattern.IsMatch(value))
            {
                Result.Increment();
                _context.Observer.InvalidPattern(instance, pattern, value);
            }
        }
    }
}
