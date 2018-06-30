namespace XmlSchemaValidator
{
    internal static class ValidationContextExtensions
    {
        public static ValidationContext Copy(this ValidationContext context)
        {
            return new ValidationContext
            {
                Observer = context?.Observer ?? new DefaultValidationObserver()
            };
        }
    }
}
