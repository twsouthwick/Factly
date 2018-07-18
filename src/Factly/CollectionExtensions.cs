// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Factly
{
    internal static class CollectionExtensions
    {
        public static T[] ToArray<T>(this IEnumerable<T> items, bool optimize)
        {
            var array = items.ToArray();

            if (optimize && array.Length == 0)
            {
                return Empty<T>();
            }

            return array;
        }

        private static T[] Empty<T>()
        {
#if FEATURE_CACHED_ARRAY
            return Array.Empty<T>();
#else
            return EmptyCache<T>.Instance;
        }

        private class EmptyCache<T>
        {
            public static T[] Instance { get; } = new T[0];
#endif
        }
    }
}
