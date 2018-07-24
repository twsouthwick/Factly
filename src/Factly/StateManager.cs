﻿// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

namespace Factly
{
    internal sealed class StateManager : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        private Dictionary<StateKey, object> _state;

        private Dictionary<StateKey, object> State
        {
            get
            {
                if (_state == null)
                {
                    _state = new Dictionary<StateKey, object>();
                }

                return _state;
            }
        }

        public TValue AddOrGet<TKey, TValue>(TKey key, Func<TKey, TValue> addFunc)
        {
            _lock.EnterUpgradeableReadLock();

            try
            {
                var dictionary = GetCache<TKey, TValue>();

                if (dictionary.TryGetValue(key, out var result))
                {
                    return result;
                }


                var newItem = addFunc(key);

                _lock.EnterWriteLock();
                try
                {
                    dictionary[key] = newItem;
                    return newItem;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
            finally
            {
                _lock.ExitUpgradeableReadLock();
            }
        }

        public void Dispose()
        {
            _lock.Dispose();
        }

        private Dictionary<TKey, TValue> GetCache<TKey, TValue>()
        {
            var key = StateKey.Create<TKey, TValue>();

            if (State.TryGetValue(key, out var result))
            {
                if (result is Dictionary<TKey, TValue> dictionary)
                {
                    return dictionary;
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                var dictionary = new Dictionary<TKey, TValue>();
                State.Add(key, dictionary);
                return dictionary;
            }
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
