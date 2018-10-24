// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Factly.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#if FEATURE_PARALLEL
using System.Threading.Tasks;
#endif

#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace Factly
{
    /// <summary>
    /// A class that contains information to validate objects as defined via <see cref="ValidatorBuilder{TOptions}"/>.
    /// </summary>
    public sealed class Validator<TState>
    {
        private readonly TypeDictionary<TypeValidator<TState>> _typeValidators;

        internal Validator(TypeDictionary<TypeValidator<TState>> typeValidators)
        {
            _typeValidators = typeValidators;
        }

#if !NO_CANCELLATION_TOKEN
        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="context"><see cref="ValidationContext"/> to pass through.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public void Validate(object item, ValidationContext context, TState state, CancellationToken token = default)
        {
            ValidateInternal(item, new ValidationContext(context), state, token);
        }

        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public void Validate(object item, TState state, CancellationToken token = default) => Validate(item, null, state, token);

#if FEATURE_PARALLEL
        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="context"><see cref="ValidationContext"/> to pass through.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public Task ValidateAsync(object item, ValidationContext context, TState state, CancellationToken token = default)
        {
            return ValidateInternalAsync(item, new ValidationContext(context), state, token);
        }

        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public Task ValidateAsync(object item, TState state, CancellationToken token = default) => ValidateAsync(item, null, state, token);
#endif
#else
        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="context"><see cref="ValidationContext"/> to pass through.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        public void Validate(object item, ValidationContext context, TState state)
        {
            var wrappedContext = new ValidationContext(context);

            ValidateInternal(item, wrappedContext, state, new CancellationToken(new ValidationContext(wrappedContext)));
        }

        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator{TState}"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="state">Optional state passed to the validation.</param>
        public void Validate(object item, TState state) => Validate(item, null, state);
#endif

        private void ValidateInternal(object item, ValidationContext context, TState state, CancellationToken token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

#if FEATURE_PARALLEL
            if (context.MaxDegreeOfParallelism != 1)
            {
                throw new ArgumentOutOfRangeException(nameof(context), context.MaxDegreeOfParallelism, SR.ParallelThreadNumberMustBeOneForNonAsync);
            }
#endif

            SingletonList.Create(item).Traverse(current => BuildItem(context, current, state), token);
        }

#if FEATURE_PARALLEL
        private Task ValidateInternalAsync(object item, ValidationContext context, TState state, CancellationToken token)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (context.MaxDegreeOfParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(context), context.MaxDegreeOfParallelism, SR.ParallelThreadNumberMustBeGreaterThan0);
            }

            return SingletonList.Create(item).TraverseAsync(current => BuildItem(context, current, state), context.MaxDegreeOfParallelism, token);
        }
#endif

        private IEnumerable<object> BuildItem(ValidationContext context, object current, TState state)
        {
            context.OnItem.Invoke(current);

            var currentType = current.GetType();

            if (_typeValidators.TryGetValue(currentType, out var type))
            {
                foreach (var constraint in type.Constraints)
                {
                    if (!constraint.Validate(current, state))
                    {
                        var error = new ValidationError(current, current, null, constraint.Id, constraint.Context);
                        context.OnError(error);
                    }
                }

                foreach (var property in type.Properties)
                {
                    var value = property.Validate(current, state, context);

                    if (value != null && property.IncludeChildren)
                    {
                        yield return value;
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
