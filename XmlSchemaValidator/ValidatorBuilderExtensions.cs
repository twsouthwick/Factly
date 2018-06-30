namespace XmlSchemaValidator
{
    public static class ValidatorBuilderExtensions
    {
        public static ValidatorBuilder WithDescendants<T>(this ValidatorBuilder builder)
        {
            return builder.WithDescendents(propertyInfo =>
            {
                return typeof(T).IsAssignableFrom(propertyInfo.PropertyType);
            });
        }
    }
}
