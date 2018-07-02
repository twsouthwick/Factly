namespace ObjectValidator
{
    internal class DefaultValidationContext : ValidationContext
    {
        private readonly ListObserver<ValidationError> _patternErrors;
        private readonly ListObserver<StructuralError> _structuralErrors;
        private readonly ListObserver<object> _items;

        public DefaultValidationContext()
        {
            _patternErrors = new ListObserver<ValidationError>();
            Errors = _patternErrors;

            _structuralErrors = new ListObserver<StructuralError>();
            StructuralErrors = _structuralErrors;

            _items = new ListObserver<object>();
            Items = _items;
        }

        public ValidationResult GetResult()
        {
            return new ValidationResult(
                _structuralErrors.Items,
                _patternErrors.Items,
                _items.Items
                );
        }
    }
}
