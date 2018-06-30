using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    internal class ValidationErrorFilterObserver : ValidationObserver
    {
        private readonly ValidationContext _context;
        private readonly ValidatorBuilder _builder;

        public ValidationErrorFilterObserver(ValidationContext context, in ValidatorBuilder builder)
        {
            _context = context;
            _builder = builder;
        }

        public ValidationResult Result { get; } = new ValidationResult();

        public void Validate(object instance)
        {
            foreach (var property in instance.GetType().GetProperties())
            {
                var regex = _builder.Pattern.GetRegex(property);
                var value = property.GetValue(instance) as string;

                InvalidPattern(instance, regex, value);
            }
        }

        public override void InvalidPattern(object instance, Regex pattern, string value)
        {
            if (pattern == null)
            {
                return;
            }

            if (value == null || !pattern.IsMatch(value))
            {
                Result.Increment();
                _context.Observer.InvalidPattern(instance, pattern, value);
            }
        }
    }
}
