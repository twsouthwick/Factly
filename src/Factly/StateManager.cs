// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;

namespace Factly
{
    /// <summary>
    /// A class to store state during the build context.
    /// </summary>
    internal sealed class StateManager
    {
        private readonly ConcurrentDictionary<StateKey, object> _state = new ConcurrentDictionary<StateKey, object>();

        /// <summary>
        /// Add or gets a value from the cache.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="key">The key for the cache.</param>
        /// <param name="generator">The generator function.</param>
        /// <returns>The retrieved or generated value.</returns>
        public TValue AddOrGet<TKey, TValue>(TKey key, Func<TKey, TValue> generator)
        {
            return GetCache<TKey, TValue>().GetOrAdd(key, generator);
        }

        private ConcurrentDictionary<TKey, TValue> GetCache<TKey, TValue>()
        {
            return (ConcurrentDictionary<TKey, TValue>)_state.GetOrAdd(StateKey.Create<TKey, TValue>(), k =>
            {
                return new ConcurrentDictionary<TKey, TValue>();
            });
        }

        private readonly struct StateKey
        {
            public StateKey(Type key, Type value)
            {
                Key = key;
                Value = value;
            }

            public Type Key { get; }

            public Type Value { get; }

            public static StateKey Create<TKey, TValue>() => new StateKey(typeof(TKey), typeof(TValue));
        }
    }
}
