using System;
using System.Collections.Generic;

namespace ObjectValidator
{
    public sealed class Validator
    {
        private readonly Dictionary<Type, TypeValidator> _typeValidators;

        internal Validator(Dictionary<Type, TypeValidator> typeValidators)
        {
            _typeValidators = typeValidators;
        }

        public void Validate<T>(T item, ValidationContext context)
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
                var current = items.Dequeue();

                if (visited.Add(current))
                {
                    context.Items?.OnNext(current);

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

            context.OnCompleted();
        }

        public ValidationResult Validate<T>(T item)
        {
            var context = new DefaultValidationContext();

            Validate(item, context);

            return context.GetResult();
        }
    }
}
