using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class TestValidationObserver : ValidationObserver
    {
        private readonly Action<object, Regex, string> _invalidPattern;

        public TestValidationObserver(
            Action<object, Regex, string> invalidPattern)
        {
            _invalidPattern = invalidPattern;
        }

        public override void InvalidPattern(object instance, PropertyInfo property, Regex pattern, string value)
        {
            _invalidPattern?.Invoke(instance, pattern, value);
        }
    }
}
