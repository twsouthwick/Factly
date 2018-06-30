namespace XmlSchemaValidator
{
    internal class DefaultValidationObserver : ValidationObserver
    {
        private readonly ValidationResult _result;

        internal DefaultValidationObserver(ValidationResult result)
        {
            _result = result;
        }

        public override void StructuralError(ValidationError error)
        {
            _result.AddStructuralError(error);
        }
    }
}
