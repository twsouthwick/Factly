// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace ObjectValidator
{
    public class TestValidationContext
    {
        public TestValidationContext()
        {
            Errors = new List<ValidationError>();
            Items = new List<object>();
            UnknownTypes = new List<Type>();
            Context = new ValidationContext
            {
                OnError = Errors.Add,
                OnItem = Items.Add,
                OnUnknownType = UnknownTypes.Add,
            };
        }

        public List<ValidationError> Errors { get; }

        public List<object> Items { get; }

        public List<Type> UnknownTypes { get; }

        public ValidationContext Context { get; }
    }
}
