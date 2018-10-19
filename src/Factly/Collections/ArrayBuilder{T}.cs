// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Factly.Collections
{
    internal struct ArrayBuilder<T>
    {
        private readonly int _maxLength;

        private int _index;
        private T[] _array;

        public ArrayBuilder(int maxLength)
        {
            _maxLength = maxLength;
            _index = 0;
            _array = null;
        }

        public void Add(T item)
        {
            if (_array == null)
            {
                _array = new T[_maxLength];
            }

            _array[_index++] = item;
        }

        public ReadonlyArray<T> Build()
        {
            if (_array == null)
            {
                return default;
            }

            return new ReadonlyArray<T>(_array, _index);
        }
    }
}
