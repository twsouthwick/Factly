using System;

namespace ObjectValidator
{
    public class ValidationException : Exception
    {
        internal ValidationException(ValidationError error)
        {
            Error = error;
        }

        public ValidationError Error { get; }
    }
}
