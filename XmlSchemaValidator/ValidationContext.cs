using System;

namespace XmlSchemaValidator
{
    public class ValidationContext
    {
        public IObserver<ValidationError> Errors { get; set; }

        public IObserver<StructuralError> StructuralErrors { get; set; }

        public IObserver<object> Items { get; set; }

        internal void OnCompleted()
        {
            Errors?.OnCompleted();
            StructuralErrors?.OnCompleted();
            Items?.OnCompleted();
        }
    }
}
