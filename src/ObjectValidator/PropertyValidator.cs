using System;
using System.Linq;
using System.Reflection;

namespace ObjectValidator
{
    internal readonly struct PropertyValidator
    {
        private readonly PropertyInfo _property;
        private readonly IConstraint[] _constraints;

        private PropertyValidator(PropertyInfo property, bool shouldDescend, IConstraint[] constraints)
        {
            _property = property;
            _constraints = constraints;
            Type = property.PropertyType;
            ShouldDescend = shouldDescend;
        }

        public void Validate(object item, ValidationProcessor processor)
        {
            var value = _property.GetValue(item);

            foreach (var constraint in _constraints)
            {
                if (constraint.Validate(item, value) is ValidationError error)
                {
                    processor.Context.Errors?.OnNext(error);
                }
            }

            if (ShouldDescend)
            {
                processor.Validate(value);
            }
        }

        public Type Type { get; }

        public bool ShouldDescend { get; }

        public static PropertyValidator Create(PropertyInfo property, ValidatorBuilder builder)
        {
            var constraints = builder.Constraints
                .Select(factory => factory(property))
                .Where(constraint => constraint != null)
                .ToArray();
            var shouldDescend = builder.IsDescendant?.Invoke(property) ?? false;

            if (constraints.Length == 0 && !shouldDescend)
            {
                return default;
            }

            return new PropertyValidator(property, shouldDescend, constraints);
        }
    }
}
