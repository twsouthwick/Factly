namespace XmlSchemaValidator
{
    public class ValidationObserver
    {
        public virtual void InvalidPattern(object instance, string expected, string actual)
        {
        }
    }
}
