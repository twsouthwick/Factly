// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Collections.Generic;

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace Factly
{
    /// <summary>
    /// Context used during the building phase of a <see cref="Validator"/>.
    /// </summary>
    public sealed class BuilderContext : IDisposable
    {
        private readonly TypeDictionary<TypeValidator>.Builder _validators;
        private readonly bool _threadSafe;
        private int _hasConstraints;

        internal BuilderContext(ValidatorBuilder builder, bool threadSafe)
        {
            _hasConstraints = 0;
            _validators = TypeDictionary<TypeValidator>.Create(builder.Types.Count);
            _threadSafe = threadSafe;

            Builder = builder;
            State = new StateManager();
        }

        internal StateManager State { get; }

        internal ValidatorBuilder Builder { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            State.Dispose();
        }

        internal IEnumerable<Type> AddItem(Type type)
        {
            var compiledType = new TypeValidator(type, this);

            Add(type, compiledType);

            foreach (var property in compiledType.Properties)
            {
                if (property.HasConstraints)
                {
#if FEATURE_PARALLEL
                    Interlocked.Exchange(ref _hasConstraints, 1);
#else
                    _hasConstraints = 1;
#endif
                }

                if (property.IncludeChildren)
                {
                    yield return property.Type;
                }
            }
        }

        internal Validator Get()
        {
            if (_hasConstraints == 0)
            {
                throw new ValidatorBuilderException(SR.NoConstraints, Errors.NoConstraintsFound, null, null);
            }

            return new Validator(_validators.ToImmutable());
        }

        private void Add(Type type, TypeValidator compiledType)
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
