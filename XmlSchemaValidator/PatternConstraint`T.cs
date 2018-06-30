using System;

namespace XmlSchemaValidator
{
    internal class PatternConstraint<T> : PatternConstraint
    {
        private readonly Func<T, string> _pattern;

        public PatternConstraint(Func<T, string> patternFunc)
        {
            _pattern = patternFunc;
        }

        protected override Type Type => typeof(T);

        protected override string GetPattern(object obj) => _pattern((T)obj);
    }
}
