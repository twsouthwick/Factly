using System;
using System.Collections.Generic;

namespace XmlSchemaValidator
{
    public sealed class Validator
    {
        private readonly Dictionary<Type, TypeValidator> _typeValidators;

        internal Validator(Dictionary<Type, TypeValidator> typeValidators)
        {
            _typeValidators = typeValidators;
        }

        public void Validate<T>(T item, ValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            new ValidationProcessor(_typeValidators, context).Validate(item);

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
