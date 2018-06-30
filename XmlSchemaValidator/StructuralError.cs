using System.Reflection;

namespace XmlSchemaValidator
{
    public class StructuralError : ValidationError
    {
        public StructuralError(string id, object instance, PropertyInfo property)
            : base(instance, property)
        {
            Id = id;
        }

        public string Id { get; }

    }
}
