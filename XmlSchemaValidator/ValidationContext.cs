using System;

namespace XmlSchemaValidator
{
    public class ValidationContext
    {
        public IObserver<PatternValidationError> PatternErrors { get; set; }

        public IObserver<StructuralError> StructuralErrors { get; set; }

        public IObserver<object> Items { get; set; }

        internal void OnCompleted()
        {
            PatternErrors?.OnCompleted();
            StructuralErrors?.OnCompleted();
            Items?.OnCompleted();
        }
    }
}
