// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if NO_CANCELLATION_TOKEN

namespace Factly
{
    /// <summary>
    /// Abstracts the notion of whether an object can be cancelled.
    /// </summary>
    internal interface ICancellable
    {
        /// <summary>
        /// Gets a value indicating whether the action can be cancelled.
        /// </summary>
        bool IsCancelled { get; }
    }
}
#endif

