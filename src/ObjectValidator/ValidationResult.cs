using System.Collections.Generic;

namespace ObjectValidator
{
    public class ValidationResult
    {
        internal ValidationResult(
            IReadOnlyCollection<ValidationError> errors,
            IReadOnlyCollection<object> items
            )
        {
            Errors = errors;
            Items = items;
        }

        public IReadOnlyCollection<ValidationError> Errors { get; }

        public IReadOnlyCollection<object> Items { get; }
    }
}
