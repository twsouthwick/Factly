namespace XmlSchemaValidator
{
    public class ValidationContext
    {
        public ValidationObserver Observer { get; set; }
    }

    internal static class ValidationContextExtensions
    {
        public static ValidationContext Copy(this ValidationContext context, ValidationResult result)
        {
            return new ValidationContext
            {
                Observer = context?.Observer ?? new DefaultValidationObserver(result)
            };
        }
    }
}
