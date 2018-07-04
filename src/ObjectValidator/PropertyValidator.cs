using System;
using System.Linq;
using System.Reflection;

namespace ObjectValidator
{
    internal readonly struct PropertyValidator
    {
        private readonly Func<object, object> _getter;
        private readonly IConstraint[] _constraints;

        private PropertyValidator(PropertyInfo property, bool shouldDescend, IConstraint[] constraints)
        {
            _getter = property.GetPropertyDelegate();
            _constraints = constraints;
            Type = property.PropertyType;
            ShouldDescend = shouldDescend;
        }

        public object Validate(object item, ValidationContext context)
        {
            var value = _getter(item);

            foreach (var constraint in _constraints)
            {
                if (constraint.Validate(item, value) is ValidationError error)
                {
                    context.Errors.Invoke(error);
                }
            }

            return value;
        }

        public Type Type { get; }

        public bool ShouldDescend { get; }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            var constraints = builder.Constraints
                .Select(factory => factory(property))
                .Where(constraint => constraint != null)
                .ToArray();
            var shouldDescend = builder.DescendantFilters.Any(t => t(property));

            if (constraints.Length == 0 && !shouldDescend)
            {
                return default;
            }

            return new PropertyValidator(property, shouldDescend, constraints);
        }
    }
}
