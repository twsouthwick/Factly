using System;
using System.Linq;

namespace ObjectValidator
{
    internal readonly struct TypeValidator
    {
        public TypeValidator(Type type, ValidatorBuilder builder)
        {
            Properties = type.GetProperties()
                .Select(p => PropertyValidator.Create(p, builder))
                .Where(p => p.Type != null)
                .ToArray();
        }

        public PropertyValidator[] Properties { get; }
    }
}
