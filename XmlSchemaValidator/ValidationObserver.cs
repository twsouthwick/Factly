using System.Reflection;
using System.Text.RegularExpressions;

namespace XmlSchemaValidator
{
    public class ValidationObserver
    {
        public virtual void StructuralError(StructuralError error)
        {
        }

        public virtual void InvalidPattern(object instance, PropertyInfo property, Regex pattern, string value)
        {
        }

        public virtual void ItemVisited(object instance)
        {
        }
    }
}
