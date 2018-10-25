// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace Factly
{
    /// <summary>
    /// An <see cref="IConstraint{TState}"/> that validates against a regular expression.
    /// </summary>
    public class PatternConstraint<TState> : IConstraint<TState>
    {
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternConstraint{TState}"/> class.
        /// </summary>
        /// <param name="regex">The regular expression of the pattern.</param>
        public PatternConstraint(Regex regex)
        {
            _regex = regex ?? throw new ArgumentNullException(nameof(regex));
        }

        /// <inheritdoc/>
        public string Id => "Pattern";

        /// <inheritdoc/>
        public object Context => _regex;

        /// <inheritdoc/>
        public void Validate(object value, ConstraintContext<TState> context)
        {
            var isValid = value is string str && _regex.IsMatch(str);

            if (!isValid)
            {
                context.RaiseError($"'{value}' does not match '{_regex}'");
            }
        }
    }
}
