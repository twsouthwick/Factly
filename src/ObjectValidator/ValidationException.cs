// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace ObjectValidator
{
    /// <summary>
    /// Exception thrown when an error is encountered while validating.
    /// </summary>
#if FEATURE_SERIALIZABLE
    [Serializable]
#endif
    public class ValidationException : Exception
    {
        internal ValidationException(ValidationError error)
        {
            Error = error;
        }

#if FEATURE_SERIALIZABLE
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class given serialization info.
        /// </summary>
        /// <param name="serializationInfo">Serialization info.</param>
        /// <param name="streamingContext">The serialization context.</param>
        protected ValidationException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
#endif

        /// <summary>
        /// Gets the error that caused the exception.
        /// </summary>
#if FEATURE_SERIALIZABLE
        [field: NonSerialized]
#endif
        public ValidationError Error { get; }
    }
}
