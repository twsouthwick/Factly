using System;

namespace XmlSchemaValidator
{
    public class ValidationContext
    {
    }

    public class ValidationResult
    {

    }

    public class Validator
    {
        private readonly ValidatorBuilder _builder;

        internal Validator(ValidatorBuilder builder)
        {
            _builder = builder;
        }

        public ValidationResult Validate<T>(T item) => Validate(default, item);

        public ValidationResult Validate<T>(ValidationContext context, T item)
        {
            return default;
        }
    }

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

    internal interface IConstraint
    {
        bool Validate(object obj);
    }

    internal interface IPatternConstraint
    {
        string GetPattern(object obj);
    }

    internal class PatternConstraint<T> : IPatternConstraint
    {
        private readonly Func<T, string> _pattern;

        public PatternConstraint(Func<T, string> patternFunc)
        {
            _pattern = patternFunc;
        }

        public string GetPattern(object obj) => _pattern((T)obj);
    }
}
