using System;
using System.Reflection;

namespace XmlSchemaValidator
{
    public readonly struct ValidatorBuilder
    {
        private ValidatorBuilder(
            PatternConstraint pattern,
            Func<PropertyInfo, bool> isDescendant
            )
        {
            Pattern = pattern;
            IsDescendant = isDescendant;
        }

        internal PatternConstraint Pattern { get; }

        internal Func<PropertyInfo, bool> IsDescendant { get; }

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder WithRegexConstraint<T>(Func<T, string> patternConstraint)
            where T : Attribute
        {
            return CreateNew(pattern: new PatternConstraint<T>(patternConstraint));
        }

        public ValidatorBuilder WithDescendents(Func<PropertyInfo, bool> isDescendant)
        {
            return CreateNew(isDescendant: isDescendant);
        }

        private ValidatorBuilder CreateNew(
            PatternConstraint pattern = null,
            Func<PropertyInfo, bool> isDescendant = null
            )
        {
            return new ValidatorBuilder(
                pattern ?? Pattern,
                isDescendant ?? IsDescendant
                );
        }

        public Validator Build() => new Validator(this);
    }
}
