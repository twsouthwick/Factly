using System.Reflection;

namespace XmlSchemaValidator
{
    public class ValidationError
    {
        internal ValidationError(string id, object instance, PropertyInfo property)
        {
            Id = id;
            Instance = instance;
            Property = property;
        }

        public string Id { get; }

        public object Instance { get; }

        public PropertyInfo Property { get; }
    }
}
