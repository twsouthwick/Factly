namespace XmlSchemaValidator
{
    public class Validator
    {
        private readonly ValidatorBuilder _builder;

        internal Validator(ValidatorBuilder builder)
        {
            _builder = builder;
        }

        public ValidationResult Validate<T>(T item) => ValidateInternal(default, item);

        public void Validate<T>(ValidationContext context, T item)
        {
            ValidateInternal(context, item);
        }

        public ValidationResult ValidateInternal<T>(ValidationContext context, T item)
        {
            var observer = new ValidationVisitor(context, _builder);

            observer.Validate(item);

            return observer.Result;
        }
    }
}
