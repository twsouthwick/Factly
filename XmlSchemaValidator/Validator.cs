namespace XmlSchemaValidator
{
    public class Validator
    {
        private readonly ValidatorBuilder _builder;

        internal Validator(ValidatorBuilder builder)
        {
            _builder = builder;
        }

        public void Validate<T>(T item, ValidationContext context)
        {
            var visitor = new ValidationProcessor(context, _builder);

            visitor.Validate(item);
        }

        public ValidationResult Validate<T>(T item)
        {
            var observer = new DefaultValidationObserver();
            var context = new ValidationContext
            {
                Observer = observer
            };

            var visitor = new ValidationProcessor(context, _builder);

            visitor.Validate(item);

            return observer.ToResult();
        }
    }
}
