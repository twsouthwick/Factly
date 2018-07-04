// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace ObjectValidator
{
    /// <summary>
    /// An exception thrown when an error occurs during building a <see cref="Validator"/>
    /// </summary>
    public class ValidatorException : Exception
    {
        internal ValidatorException(string message, string id, Type type, PropertyInfo property)
            : base(message)
        {
            Id = id;
            Type = type;
            Property = property;
        }

        /// <summary>
        /// Gets the error id. A list of known errors are available in <see cref="Errors"/>
        /// </summary>
        public string Id { get; }

#pragma warning disable CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'
        /// <summary>
        /// Gets the type that caused the exception to be thrown
        /// </summary>
        public Type Type { get; }
#pragma warning restore CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'

        /// <summary>
        /// Gets the property that caused the exception to be thrown
        /// </summary>
        public PropertyInfo Property { get; }
    }
}
