// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Factly
{
    internal struct ArrayBuilder<T>
    {
#if !FEATURE_CACHED_ARRAY
        private static readonly T[] Instance = new T[0];
#endif
        private List<T> _list;

        public void Add(T item)
        {
            if (_list == null)
            {
                _list = new List<T>();
            }

            _list.Add(item);
        }

        public T[] ToArray()
        {
            if (_list == null)
            {
                return Empty();
            }

            return _list.ToArray();
        }

        private static T[] Empty()
        {
#if FEATURE_CACHED_ARRAY
            return Array.Empty<T>();
#else
            return Instance;
#endif
        }
    }
}
