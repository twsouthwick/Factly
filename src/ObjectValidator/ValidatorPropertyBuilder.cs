using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectValidator
{
    public sealed class ValidatorPropertyBuilder<TType>
    {
        private readonly ValidatorBuilder _builder;

        internal ValidatorPropertyBuilder(ValidatorBuilder builder)
        {
            _builder = builder;
            _builder.AddKnownType<TType>();
        }

        public ValidatorPropertyBuilder<TType> AddProperty<TProperty>(Expression<Func<TType, TProperty>> memberExpression)
        {
            if (memberExpression.Body is MemberExpression member && member.Member is PropertyInfo property)
            {
                _builder.AddPropertyFilter(p => p == property);
                return this;
            }

            throw new InvalidOperationException(SR.ExpressionPropertyRequired);
        }
    }
}
