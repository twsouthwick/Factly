namespace XmlSchemaValidator
{
    public class ValidationResult
    {
        public int TotalErrors { get; private set; }

        internal void Increment() => TotalErrors++;
    }
}
