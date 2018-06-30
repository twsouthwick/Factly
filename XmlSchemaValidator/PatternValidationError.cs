using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class PatternValidationError : ValidationError
    {
        internal PatternValidationError(object instance, PropertyInfo property, Regex pattern, object value)
            : base(instance, property)
        {
            Pattern = pattern;
            Value = value;
        }

        public object Value { get; }

        public Regex Pattern { get; }
    }
}
