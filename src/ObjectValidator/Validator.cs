// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;

#if !NO_CANCELLATION_TOKEN
using CancellationToken = System.Threading.CancellationToken;
#else
using CancellationToken = ObjectValidator.Validator.InternalCancellationToken;
#endif

namespace ObjectValidator
{
    public sealed class Validator
    {
        private readonly Dictionary<Type, TypeValidator> _typeValidators;

        internal Validator(Dictionary<Type, TypeValidator> typeValidators)
        {
            _typeValidators = typeValidators;
        }

#if !NO_CANCELLATION_TOKEN
        public void Validate(object item, ValidationContext context, CancellationToken token = default)
        {
            ValidateInternal(item, new ValidationContext(context), token);
        }

        public void Validate(object item, CancellationToken token = default) => Validate(item, null, token);
#else
#pragma warning disable SA1201 // Elements should appear in the correct order
        internal readonly struct InternalCancellationToken
        {
            private readonly ValidationContext _context;

            public InternalCancellationToken(ValidationContext context)
            {
                _context = context;
            }

            public void ThrowIfCancellationRequested()
            {
                if (_context.IsCancelled)
                {
                    throw new OperationCanceledException();
                }
            }
        }

        public void Validate(object item, ValidationContext context)
        {
            var wrappedContext = new ValidationContext(context);

            ValidateInternal(item, wrappedContext, new CancellationToken(new ValidationContext(wrappedContext)));
        }

        public void Validate(object item) => Validate(item, null);
#pragma warning restore SA1201 // Elements should appear in the correct order
#endif

        private void ValidateInternal(object item, ValidationContext context, CancellationToken token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            var visited = new HashSet<object>();
            var items = new Queue<object>();
            items.Enqueue(item);

            while (items.Count > 0)
            {
                token.ThrowIfCancellationRequested();

                var current = items.Dequeue();

                if (visited.Add(current))
                {
                    context.OnItem.Invoke(current);

                    var currentType = current.GetType();

                    if (_typeValidators.TryGetValue(currentType, out var type))
                    {
                        foreach (var property in type.Properties)
                        {
                            var value = property.Validate(current, context);

                            if (value != null && property.ShouldFollow)
                            {
                                items.Enqueue(value);
                            }
                        }
                    }
                    else
                    {
                        context.OnUnknownType(currentType);
                    }
                }
            }
        }
    }
}
