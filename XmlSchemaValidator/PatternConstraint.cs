using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    internal abstract class PatternConstraint
    {
        protected abstract string GetPattern(object obj);

        protected abstract Type Type { get; }

        public Regex GetRegex(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute(Type);

            if (attribute is null)
            {
                return null;
            }

            return new Regex(GetPattern(attribute), RegexOptions.Compiled);
        }
    }
}
