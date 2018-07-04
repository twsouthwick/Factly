// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ObjectValidator
{
    /// <summary>
    /// Exception thrown when an error is encountered while validating
    /// </summary>
    public class ValidationException : Exception
    {
        internal ValidationException(ValidationError error)
        {
            Error = error;
        }

        /// <summary>
        /// Gets the error that caused the exception
        /// </summary>
        public ValidationError Error { get; }
    }
}
