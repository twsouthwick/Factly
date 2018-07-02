using System.Reflection;

namespace ObjectValidator
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
