// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if !NO_CANCELLATION_TOKEN
using System.Threading;
#endif

namespace ObjectValidator
{
    /// <summary>
    /// A class that contains information to validate objects as defined via <see cref="ValidatorBuilder"/>.
    /// </summary>
    [DebuggerTypeProxy(typeof(ValidatorDebugProxy))]
    public sealed class Validator
    {
        private readonly Dictionary<Type, TypeValidator> _typeValidators;

        internal Validator(Dictionary<Type, TypeValidator> typeValidators)
        {
            _typeValidators = typeValidators;
        }

#if !NO_CANCELLATION_TOKEN
        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="context"><see cref="ValidationContext"/> to pass through.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public void Validate(object item, ValidationContext context, CancellationToken token = default)
        {
            ValidateInternal(item, new ValidationContext(context), token);
        }

        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="token">An optional <see cref="CancellationToken"/>.</param>
        public void Validate(object item, CancellationToken token = default) => Validate(item, null, token);
#else
        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator"/> and <paramref name="context"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        /// <param name="context"><see cref="ValidationContext"/> to pass through.</param>
        public void Validate(object item, ValidationContext context)
        {
            var wrappedContext = new ValidationContext(context);

            ValidateInternal(item, wrappedContext, new CancellationToken(new ValidationContext(wrappedContext)));
        }

        /// <summary>
        /// Validates an item and its object graph according to that defined in <see cref="Validator"/>.
        /// </summary>
        /// <param name="item">Item to validate.</param>
        public void Validate(object item) => Validate(item, null);
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

            IEnumerable<object> BuildItem(object current)
            {
                context.OnItem.Invoke(current);

                var currentType = current.GetType();

                if (_typeValidators.TryGetValue(currentType, out var type))
                {
                    foreach (var property in type.Properties)
                    {
                        var value = property.Validate(current, context);

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

            SingletonList.Create(item).Traverse(BuildItem, token);
        }

#pragma warning disable CA1812
        internal class ValidatorDebugProxy
#pragma warning restore CA1812
        {
            private readonly Validator _validator;

            public ValidatorDebugProxy(Validator validator)
            {
                _validator = validator;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public ICollection<TypeValidator> Validators => _validator._typeValidators.Values;
        }
    }
}
