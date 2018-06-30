using System;
using System.Reflection;

namespace XmlSchemaValidator
{
    public readonly struct ValidatorBuilder
    {
        private ValidatorBuilder(
            PatternConstraint pattern,
            Func<PropertyInfo, bool> recursiveHandler
            )
        {
            Pattern = pattern;
            RecursiveHandler = recursiveHandler;
        }

        internal PatternConstraint Pattern { get; }

        internal Func<PropertyInfo, bool> RecursiveHandler { get; }

        public static ValidatorBuilder Create() => default;

        public ValidatorBuilder AddRegexConstraint<T>(Func<T, string> patternFunc)
            where T : Attribute
        {
            return CreateNew(pattern: new PatternConstraint<T>(patternFunc));
        }

        public ValidatorBuilder AddRecursiveDescent(Func<PropertyInfo, bool> recursiveHandler)
        {
            return CreateNew(recursiveHandler: recursiveHandler);
        }

        private ValidatorBuilder CreateNew(
            PatternConstraint pattern = null,
            Func<PropertyInfo, bool> recursiveHandler = null
            )
        {
            return new ValidatorBuilder(
                pattern ?? Pattern,
                recursiveHandler ?? RecursiveHandler
                );
        }

        public Validator Build() => new Validator(this);
    }
}
