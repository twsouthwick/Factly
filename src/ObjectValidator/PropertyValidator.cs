using System;
using System.Linq;
using System.Reflection;

namespace ObjectValidator
{
    internal readonly struct PropertyValidator
    {
        private readonly Func<object, object> _getter;
        private readonly IConstraint[] _constraints;

        private PropertyValidator(PropertyInfo property, bool shouldFollow, IConstraint[] constraints)
        {
            _getter = property.GetPropertyDelegate();
            _constraints = constraints;
            Type = property.PropertyType;
            ShouldFollow = shouldFollow;
        }

        public object Validate(object item, ValidationContext context)
        {
            var value = _getter(item);

            foreach (var constraint in _constraints)
            {
                constraint.Validate(item, value, context);
            }

            return value;
        }

        public Type Type { get; }

        public bool ShouldFollow { get; }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            var constraints = builder.Constraints
                .Select(factory => factory(property))
                .Where(constraint => constraint != null)
                .ToArray();
            var shouldFollow = builder.PropertyFilters.Any(t => t(property));

            if (constraints.Length == 0 && !shouldFollow)
            {
                return default;
            }

            return new PropertyValidator(property, shouldFollow, constraints);
        }
    }
}
