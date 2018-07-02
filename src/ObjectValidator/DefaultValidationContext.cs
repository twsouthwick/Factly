namespace ObjectValidator
{
    internal class DefaultValidationContext : ValidationContext
    {
        private readonly ListObserver<ValidationError> _patternErrors;
        private readonly ListObserver<object> _items;

        public DefaultValidationContext()
        {
            _patternErrors = new ListObserver<ValidationError>();
            Errors = _patternErrors;

            _items = new ListObserver<object>();
            Items = _items;
        }

        public ValidationResult GetResult()
        {
            return new ValidationResult(
                _patternErrors.Items,
                _items.Items
                );
        }
    }
}
