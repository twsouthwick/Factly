using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace ObjectValidator
{
    public readonly struct ValidatorBuilder
    {
        private readonly ImmutableHashSet<Type> _types;
        private readonly ImmutableList<Func<PropertyInfo, IConstraint>> _constraints;

        private ValidatorBuilder(
            PatternConstraint pattern,
            Func<PropertyInfo, bool> isDescendant,
            ImmutableHashSet<Type> types,
            ImmutableList<Func<PropertyInfo, IConstraint>> constraints
            )
        {
            Pattern = pattern;
            IsDescendant = isDescendant;
            _types = types ?? ImmutableHashSet.Create<Type>();
            _constraints = constraints ?? ImmutableList.Create<Func<PropertyInfo, IConstraint>>();
        }

        internal PatternConstraint Pattern { get; }

        internal Func<PropertyInfo, bool> IsDescendant { get; }

        internal ImmutableHashSet<Type> Types => _types ?? ImmutableHashSet.Create<Type>();

        internal ImmutableList<Func<PropertyInfo, IConstraint>> Constraints => _constraints ?? ImmutableList.Create<Func<PropertyInfo, IConstraint>>();

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder AddConstraint(Func<PropertyInfo, IConstraint> constraint) => Update(constraints: Constraints.Add(constraint));

        public ValidatorBuilder WithDescendents(Func<PropertyInfo, bool> isDescendant) => Update(isDescendant: isDescendant);

        public ValidatorBuilder AddKnownTypes(IEnumerable<Type> types)
        {
            var builder = Types.ToBuilder();

            foreach (var type in types)
            {
                builder.Add(type);
            }

            return Update(types: builder.ToImmutable());
        }

        public ValidatorBuilder AddKnownType(Type type)
        {
            return Update(types: Types.Add(type));
        }

        private ValidatorBuilder Update(
            PatternConstraint pattern = null,
            Func<PropertyInfo, bool> isDescendant = null,
            ImmutableHashSet<Type> types = default,
            ImmutableList<Func<PropertyInfo, IConstraint>> constraints = null
            )
        {
            return new ValidatorBuilder(
                pattern ?? Pattern,
                isDescendant ?? IsDescendant,
                types ?? Types,
                constraints ?? Constraints
                );
        }

        public Validator Build()
        {
            if (Types.IsEmpty)
            {
                throw new ValidatorException("Must declare types for compilation", StructuralErrors.NoTypes, null, null);
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
