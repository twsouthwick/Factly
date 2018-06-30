using System;

namespace XmlSchemaValidator
{
    public readonly struct ValidatorBuilder
    {
        private ValidatorBuilder(PatternConstraint pattern)
        {
            Pattern = pattern;
        }

        internal PatternConstraint Pattern { get; }

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder AddRegexConstraint<T>(Func<T, string> patternFunc)
            where T : Attribute
        {
            return CreateNew(new PatternConstraint<T>(patternFunc));
        }

        private ValidatorBuilder CreateNew(
            PatternConstraint pattern = null)
        {
            return new ValidatorBuilder(
                pattern: pattern ?? Pattern
                );
        }

        public Validator Build() => new Validator(this);
    }
}
