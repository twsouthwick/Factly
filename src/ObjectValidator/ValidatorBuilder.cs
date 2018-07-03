using System;
using System.Collections.Generic;
using System.Reflection;

namespace ObjectValidator
{
    public sealed class ValidatorBuilder
    {
        private ValidatorBuilder()
        {
            Types = new HashSet<Type>();
            Constraints = new List<Func<PropertyInfo, IConstraint>>();
        }

        internal PatternConstraint Pattern { get; private set; }

        internal Func<PropertyInfo, bool> IsDescendant { get; private set; }

        internal HashSet<Type> Types { get; }

        internal List<Func<PropertyInfo, IConstraint>> Constraints { get; }

        public static ValidatorBuilder Create() => new ValidatorBuilder();

        public ValidatorBuilder AddConstraint(Func<PropertyInfo, IConstraint> constraint)
        {
            Constraints.Add(constraint);
            return this;
        }

        public ValidatorBuilder AddDescendantFilter(Func<PropertyInfo, bool> isDescendant)
        {
            IsDescendant = isDescendant;
            return this;
        }

        public ValidatorBuilder AddKnownTypes(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                Types.Add(type);
            }

            return this;
        }

        public ValidatorBuilder AddKnownType(Type type)
        {
            Types.Add(type);
            return this;
        }

        public Validator Build()
        {
            if (Types.Count == 0)
            {
                throw new ValidatorException("Must declare types for compilation", Errors.NoTypes, null, null);
            }

            var visited = new HashSet<Type>();
            var left = new Queue<Type>(Types);
            var validators = new Dictionary<Type, TypeValidator>();

            while (left.Count > 0)
            {
                var type = left.Dequeue();
                if (visited.Add(type))
                {
                    var compiledType = new TypeValidator(type, this);
                    validators.Add(type, compiledType);

                    foreach (var property in compiledType.Properties)
                    {
                        if (property.ShouldDescend)
                        {
                            left.Enqueue(property.Type);
                        }
                    }
                }
            }

            return new Validator(validators);
        }
    }
}
