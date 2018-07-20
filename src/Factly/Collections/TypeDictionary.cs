// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Factly.Collections
{
    internal class TypeDictionary<TValue>
    {
        private readonly Dictionary<TypeKey, TValue> _dictionary;

        public TypeDictionary(int size)
        {
            _dictionary = new Dictionary<TypeKey, TValue>(size);
        }

        public ICollection<TValue> Values => _dictionary.Values;

        public void Add(Type type, TValue value)
        {
            _dictionary.Add(type, value);
        }

        public bool TryGetValue(Type currentType, out TValue value)
        {
            return _dictionary.TryGetValue(currentType, out value);
        }

        private readonly struct TypeKey : IEquatable<TypeKey>, IEquatable<Type>
        {
            public TypeKey(Type type)
            {
                Type = type;
            }

            public Type Type { get; }

            public static implicit operator TypeKey(Type value) => new TypeKey(value);

            public static implicit operator Type(TypeKey value) => value.Type;

            public bool Equals(TypeKey other) => ReferenceEquals(Type, other.Type);

            public bool Equals(Type type) => ReferenceEquals(Type, type);

            public override bool Equals(object obj)
            {
                if (obj is TypeKey key)
                {
                    return Equals(key);
                }
                else if (obj is Type type)
                {
                    return ReferenceEquals(Type, type);
                }

                return false;
            }

            public override int GetHashCode() => Type.GetHashCode();
        }
    }
}
