using System;
using System.Collections.Generic;

namespace ObjectValidator
{
    internal readonly struct ValidationProcessor
    {
        private readonly Dictionary<Type, TypeValidator> _typeValidators;
        private readonly HashSet<object> _visited;

        public ValidationProcessor(Dictionary<Type, TypeValidator> typeValidators, ValidationContext context)
        {
            _typeValidators = typeValidators;
            _visited = new HashSet<object>();
            Context = context;
        }

        public ValidationContext Context { get; }

        public void Validate(object item)
        {
            if (item == null)
            {
                return;
            }

            if (!_visited.Add(item))
            {
                return;
            }

            Context.Items?.OnNext(item);

            if (_typeValidators.TryGetValue(item.GetType(), out var validator))
            {
                validator.Validate(item, this);
            }
        }
    }
}
