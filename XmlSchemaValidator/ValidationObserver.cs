using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class ValidationObserver
    {
        public virtual void InvalidPattern(object instance, Regex pattern, string value)
        {
        }
    }
}
