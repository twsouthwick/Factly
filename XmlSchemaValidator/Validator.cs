namespace XmlSchemaValidator
{
    public class Validator
    {
        private readonly ValidatorBuilder _builder;

        internal Validator(ValidatorBuilder builder)
        {
            _builder = builder;
        }

        public ValidationResult Validate<T>(T item) => ValidateInternal(item, default);

        public void Validate<T>(T item, ValidationContext context)
        {
            ValidateInternal(item, context);
        }

        private ValidationResult ValidateInternal<T>(T item, ValidationContext context)
        {
            var observer = new ValidationVisitor(context, _builder);

            observer.Validate(item);

            return observer.Result;
        }
    }
}
