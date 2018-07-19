// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Factly
{
    internal class ConcurrentHashSet<T> : ConcurrentDictionary<T, byte>, IEnumerable<T>
    {
        public bool Add(T item) => TryAdd(item, 0);

        public bool Remove(T item) => TryRemove(item, out _);

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => Keys.GetEnumerator();
    }
}
