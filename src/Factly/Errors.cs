// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly
{
    /// <summary>
    /// A collection of known errors.
    /// </summary>
    public static class Errors
    {
        /// <summary>
        /// Attempted to build validator without specifying types.
        /// </summary>
        public const string NoTypes = "E0001";

        /// <summary>
        /// An unknown type was encountered.
        /// </summary>
        public const string UnknownType = "E0002";

        /// <summary>
        /// Unsupported type provided for a typed constraint.
        /// </summary>
        public const string UnsupportedTypeForConstraint = "E0003";
    }
}
