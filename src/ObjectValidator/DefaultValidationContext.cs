using System.Collections.Generic;

namespace ObjectValidator
{
    internal class DefaultValidationContext : ValidationContext
    {
        private readonly List<ValidationError> _errors = new List<ValidationError>();
        private readonly List<object> _items = new List<object>();

        public DefaultValidationContext()
        {
            Errors = _errors.Add;
            Items = _items.Add;
        }

        public ValidationResult GetResult()
        {
            return new ValidationResult(_errors, _items);
        }
    }
}
