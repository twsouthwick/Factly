// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ObjectValidator
{
    /// <summary>
    /// A validator builder for properties for the Type <typeparamref name="TType"/>.
    /// </summary>
    /// <typeparam name="TType">Type to build property filters for.</typeparam>
    public sealed class ValidatorPropertyBuilder<TType>
    {
        private readonly ValidatorBuilder _builder;

        internal ValidatorPropertyBuilder(ValidatorBuilder builder)
        {
            _builder = builder;
            _builder.AddKnownType<TType>();
        }

        /// <summary>
        /// Add a property filter to <see cref="ValidatorBuilder"/> for <typeparamref name="TProperty"/>.
        /// </summary>
        /// <typeparam name="TProperty">Property to add a filter for.</typeparam>
        /// <param name="memberExpression">Expression to identify the property.</param>
        /// <returns>The current <see cref="ValidatorPropertyBuilder{TType}"/>.</returns>
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
