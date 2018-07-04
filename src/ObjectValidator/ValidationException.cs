// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ObjectValidator
{
    public class ValidationException : Exception
    {
        internal ValidationException(ValidationError error)
        {
            Error = error;
        }

        public ValidationError Error { get; }
    }
}
