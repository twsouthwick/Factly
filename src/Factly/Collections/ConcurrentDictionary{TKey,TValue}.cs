// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Collections.Concurrent
{
    internal class ConcurrentDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> addFunc)
        {
            lock (_values)
            {
                if (_values.TryGetValue(key, out var value))
                {
                    return value;
                }
                else
                {
                    var result = addFunc(key);
                    _values[key] = result;
                    return result;
                }
            }
        }
    }
}
