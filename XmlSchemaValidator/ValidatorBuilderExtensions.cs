namespace XmlSchemaValidator
{
    public static class ValidatorBuilderExtensions
    {
        public static ValidatorBuilder AddRecursiveDescent<T>(this ValidatorBuilder builder)
        {
            return builder.WithDescendents(propertyInfo =>
            {
                return typeof(T) == propertyInfo.PropertyType;
            });
        }
    }
}
