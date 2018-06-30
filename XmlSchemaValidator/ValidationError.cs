using System.Reflection;

namespace XmlSchemaValidator
{
    public class ValidationError
    {
        internal ValidationError(object instance, PropertyInfo property)
        {
            Instance = instance;
            Property = property;
        }

        public object Instance { get; }

        public PropertyInfo Property { get; }
    }
}
