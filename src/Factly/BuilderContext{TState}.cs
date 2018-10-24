// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace Factly
{
    /// <summary>
    /// Context used during the building phase of a <see cref="Validator{TState}"/>.
    /// </summary>
    public sealed class BuilderContext<TState> : IDisposable
    {
        private readonly TypeDictionary<TypeValidator<TState>>.Builder _validators;
        private readonly StateManager _state;
        private readonly bool _threadSafe;

        private int _hasConstraints;
        private bool _isDisposed;

        internal BuilderContext(ValidatorBuilder<TState> builder, bool threadSafe)
        {
            _hasConstraints = 0;
            _validators = TypeDictionary<TypeValidator<TState>>.Create(builder.Types.Count);
            _threadSafe = threadSafe;
            _state = new StateManager();

            Builder = builder;
        }

        internal ValidatorBuilder<TState> Builder { get; }

        /// <summary>
        /// Gets or sets a value in a context specific data store.
        /// </summary>
        /// <typeparam name="TKey">The type for the key.</typeparam>
        /// <typeparam name="TValue">The type for the value.</typeparam>
        /// <param name="key">The key used to identify the value.</param>
        /// <param name="generator">A generator to generate the value if it is not already present in the store.</param>
        /// <returns>The stored value if exists, otherwise the generated value.</returns>
        public TValue GetOrSetState<TKey, TValue>(TKey key, Func<TKey, TValue> generator)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(BuilderContext<TState>));
            }

            return _state.AddOrGet(key, generator);
        }

        void IDisposable.Dispose()
        {
            _isDisposed = true;
        }

        internal IEnumerable<Type> AddItem(Type type)
        {
            var compiledType = new TypeValidator<TState>(type, this);

            Add(type, compiledType);

            if (compiledType.Constraints.Any())
            {
                SetConstraints();
            }

            foreach (var property in compiledType.Properties)
            {
                if (property.HasConstraints)
                {
                    SetConstraints();
                }

                if (property.IncludeChildren)
                {
                    yield return property.Type;
                }
            }
        }

        internal Validator<TState> Get()
        {
            if (_hasConstraints == 0)
            {
                throw new ValidatorBuilderException(SR.NoConstraints, Errors.NoConstraintsFound, null, null);
            }

            return new Validator<TState>(_validators.ToImmutable());
        }

        private void SetConstraints()
        {
#if FEATURE_PARALLEL
            Interlocked.Exchange(ref _hasConstraints, 1);
#else
            _hasConstraints = 1;
#endif
        }

        private void Add(Type type, TypeValidator<TState> compiledType)
        {
            if (_threadSafe)
            {
                lock (_validators)
                {
                    _validators.Add(type, compiledType);
                }
            }
            else
            {
                _validators.Add(type, compiledType);
            }
        }
    }
}
