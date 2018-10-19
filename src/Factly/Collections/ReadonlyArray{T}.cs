// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace Factly.Collections
{
    [DebuggerDisplay("Length = {Length}")]
    internal readonly struct ReadonlyArray<T>
    {
        private readonly T[] _array;

        public ReadonlyArray(T[] array, int length)
        {
            _array = array;
            Length = length;
        }

        public int Length { get; }

        public Enumerator GetEnumerator() => new Enumerator(this);

        public struct Enumerator
        {
            private readonly ReadonlyArray<T> _array;
            private int _index;

            public Enumerator(ReadonlyArray<T> array)
            {
                _array = array;
                _index = -1;
            }

            public ref T Current => ref _array._array[_index];

            public bool MoveNext() => ++_index < _array.Length;
        }
    }
}
