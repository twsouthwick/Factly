using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class DefaultValidationObserver : ValidationObserver
    {
        private readonly List<StructuralError> _structuralErrors;
        private readonly List<ValidationError> _errors;
        private readonly List<object> _items;

        public DefaultValidationObserver()
        {
            _structuralErrors = new List<StructuralError>();
            _errors = new List<ValidationError>();
            _items = new List<object>();
        }

        public IReadOnlyCollection<ValidationError> StructuralErrors => _structuralErrors;

        public override void StructuralError(StructuralError error)
        {
            _structuralErrors.Add(error);
        }

        public override void InvalidPattern(object instance, PropertyInfo property, Regex pattern, string value)
        {
            _errors.Add(new PatternValidationError(instance, property, pattern));
        }

        public override void ItemVisited(object instance)
        {
            _items.Add(instance);
        }

        public ValidationResult ToResult()
        {
            return new ValidationResult(_structuralErrors, _errors, _items);
        }
    }
}
