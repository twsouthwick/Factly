namespace XmlSchemaValidator
{
    public class Validator
    {
        private readonly ValidatorBuilder _builder;

        internal Validator(ValidatorBuilder builder)
        {
            _builder = builder;
        }

        public ValidationResult Validate<T>(T item) => Validate(default, item);

        public ValidationResult Validate<T>(ValidationContext context, T item)
        {
            return default;
        }
    }
}
