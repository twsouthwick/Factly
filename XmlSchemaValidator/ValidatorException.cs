using System;
using System.Reflection;

namespace XmlSchemaValidator
{
    public class ValidatorException : Exception
    {
        internal ValidatorException(string message, string id, Type type, PropertyInfo property)
            : base(message)
        {
            Id = id;
            Type = type;
            Property = property;
        }

        public string Id { get; }

        public Type Type { get; }

        public PropertyInfo Property { get; }
    }
}
