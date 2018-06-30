using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class PatternValidationError : ValidationError
    {
        internal PatternValidationError(object instance, PropertyInfo property, Regex pattern)
            : base(instance, property)
        {
            Pattern = pattern;
        }

        public Regex Pattern { get; }
    }
}
