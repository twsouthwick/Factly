// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace ObjectValidator
{
    public class ValidatorException : Exception
    {
        internal ValidatorException(string message, string id, Type type, PropertyInfo property)
            : base(message)
        {
            Id = id;
            Type = type;
            Property = property;
        }

        public string Id { get; }

#pragma warning disable CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'
        public Type Type { get; }
#pragma warning restore CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'

        public PropertyInfo Property { get; }
    }
}
