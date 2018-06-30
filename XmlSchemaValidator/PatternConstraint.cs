using System;

namespace XmlSchemaValidator
{
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
