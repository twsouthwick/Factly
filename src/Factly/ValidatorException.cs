// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// An exception thrown when an error occurs during building a <see cref="Validator"/>.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ValidatorException : Exception
    {
        internal ValidatorException(string message, string id, Type type, PropertyInfo property)
            : base(message)
        {
            Id = id;
            Type = type;
            Property = property;
        }

#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatorException"/> class given serialization info.
        /// </summary>
        /// <param name="serializationInfo">Serialization info.</param>
        /// <param name="streamingContext">The serialization context.</param>
        protected ValidatorException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
#endif

        /// <summary>
        /// Gets the error id. A list of known errors are available in <see cref="Errors"/>.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [field: NonSerialized]
#endif
        public string Id { get; }

#pragma warning disable CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'
        /// <summary>
        /// Gets the type that caused the exception to be thrown.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [field: NonSerialized]
#endif
        public Type Type { get; }
#pragma warning restore CA1721 // The property name 'Type' is confusing given the existence of method 'GetType'

        /// <summary>
        /// Gets the property that caused the exception to be thrown.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [field: NonSerialized]
#endif
        public PropertyInfo Property { get; }
    }
}
