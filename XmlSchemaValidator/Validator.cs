using System;

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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var visitor = new ValidationProcessor(context, _builder);

            visitor.Validate(item);
            context.OnCompleted();
        }

        public ValidationResult Validate<T>(T item)
        {
            var context = new DefaultValidationContext();

            Validate(item, context);

            return context.GetResult();
        }
    }
}
