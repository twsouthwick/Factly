using System.Collections.Generic;

namespace ObjectValidator
{
    public class ValidationResult
    {
        internal ValidationResult(
            IReadOnlyCollection<StructuralError> structuralErrors,
            IReadOnlyCollection<ValidationError> errors,
            IReadOnlyCollection<object> items
            )
        {
            StructuralErrors = structuralErrors;
            Errors = errors;
            Items = items;
        }

        public IReadOnlyCollection<StructuralError> StructuralErrors { get; }

        public IReadOnlyCollection<ValidationError> Errors { get; }

        public IReadOnlyCollection<object> Items { get; }
    }
}
