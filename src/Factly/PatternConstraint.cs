﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Factly
{
    /// <summary>
    /// An <see cref="IConstraint"/> that validates agains a regular expression.
    /// </summary>
    public class PatternConstraint : IConstraint
    {
        private readonly Regex _regex;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatternConstraint"/> class.
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
        public virtual bool Validate(object value)
        {
            return value is string str && _regex.IsMatch(str);
        }
    }
}
