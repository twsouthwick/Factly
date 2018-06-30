using System;

namespace XmlSchemaValidator
{
    public readonly struct ValidatorBuilder
    {
        private readonly IPatternConstraint _pattern;

        private ValidatorBuilder(IPatternConstraint pattern)
        {
            _pattern = pattern;
        }

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder AddRegexConstraint<T>(Func<T, string> patternFunc)
            where T : Attribute
        {
            return CreateNew(new PatternConstraint<T>(patternFunc));
        }

        private ValidatorBuilder CreateNew(
            IPatternConstraint pattern = null)
        {
            return new ValidatorBuilder(
                pattern: pattern ?? _pattern
                );
        }

        public Validator Build() => new Validator(this);
    }
}
