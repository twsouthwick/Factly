using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace XmlSchemaValidator
{
    public readonly struct ValidatorBuilder
    {
        private readonly ImmutableHashSet<Type> _types;

        private ValidatorBuilder(
            PatternConstraint pattern,
            Func<PropertyInfo, bool> isDescendant,
            ImmutableHashSet<Type> types
            )
        {
            Pattern = pattern;
            IsDescendant = isDescendant;
            _types = types ?? ImmutableHashSet.Create<Type>();
        }

        internal PatternConstraint Pattern { get; }

        internal Func<PropertyInfo, bool> IsDescendant { get; }

        internal ImmutableHashSet<Type> Types => _types ?? ImmutableHashSet.Create<Type>();

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder WithRegexConstraint<T>(Func<T, string> patternConstraint)
            where T : Attribute
        {
            return Update(pattern: new PatternConstraint<T>(patternConstraint));
        }

        public ValidatorBuilder WithDescendents(Func<PropertyInfo, bool> isDescendant)
        {
            return Update(isDescendant: isDescendant);
        }

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
            ImmutableHashSet<Type> types = default
            )
        {
            return new ValidatorBuilder(
                pattern ?? Pattern,
                isDescendant ?? IsDescendant,
                types ?? Types
                );
        }

        public Validator Build() => new ValidatorCompiler(this).Compile();


        private readonly struct ValidatorCompiler
        {
            private readonly ValidatorBuilder _builder;
            private readonly HashSet<Type> _visited;
            private readonly Dictionary<Type, TypeValidator> _typeValidators;

            public ValidatorCompiler(ValidatorBuilder builder)
            {
                _builder = builder;
                _visited = new HashSet<Type>();
                _typeValidators = new Dictionary<Type, TypeValidator>();
            }

            public Validator Compile()
            {
                if (_builder.Types.IsEmpty)
                {
                    throw new ValidatorException("Must declare types for compilation", StructuralErrors.NoTypes, null, null);
                }

                foreach (var type in _builder.Types)
                {
                    Compile(type);
                }

                return new Validator(_typeValidators);
            }

            private void Compile(Type type)
            {
                if (!_visited.Add(type))
                {
                    return;
                }

                var compiledType = new TypeValidator(type, _builder);
                _typeValidators.Add(type, compiledType);

                foreach (var property in compiledType.Properties)
                {
                    if (property.ShouldDescend)
                    {
                        Compile(property.Type);
                    }
                }
            }
        }
    }
}
