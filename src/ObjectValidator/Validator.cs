using System;
using System.Collections.Generic;
using System.Threading;

#if FEATURE_CANCELLATION_TOKEN
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

#if FEATURE_CANCELLATION_TOKEN
        public void Validate(object item, ValidationContext context, CancellationToken token = default)
        {
            ValidateInternal(item, context, token);
        }
#else
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
            ValidateInternal(item, context, new InternalCancellationToken(context));
        }
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
                    context.Items?.Invoke(current);

                    if (_typeValidators.TryGetValue(current.GetType(), out var type))
                    {
                        foreach (var property in type.Properties)
                        {
                            var value = property.Validate(current, context);

                            if (value != null && property.ShouldDescend)
                            {
                                items.Enqueue(value);
                            }
                        }
                    }
                    else
                    {
                        throw new ValidatorException("Unknown type", Errors.UnknownType, current.GetType(), null);
                    }
                }
            }
        }

        public ValidationResult Validate(object item)
        {
            var context = new DefaultValidationContext();

            Validate(item, context);

            return context.GetResult();
        }
    }
}
