using System.Collections.Generic;

namespace XmlSchemaValidator
{
    public class ValidationResult
    {
        private readonly List<ValidationError> _structuralErrors = new List<ValidationError>();

        public IReadOnlyCollection<ValidationError> StructuralErrors => _structuralErrors;

        public int TotalErrors { get; private set; }

        public int ObjectsTested { get; internal set; }

        internal void Increment() => TotalErrors++;

        internal void AddStructuralError(ValidationError error) => _structuralErrors.Add(error);
    }
}
