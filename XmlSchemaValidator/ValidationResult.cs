namespace XmlSchemaValidator
{
    public class ValidationResult
    {
        public int TotalErrors { get; private set; }

        public int ObjectsTested { get; internal set; }

        internal void Increment() => TotalErrors++;
    }
}
