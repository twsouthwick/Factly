using System;

namespace ObjectValidator
{
    internal class DelegateConstraint : IConstraint
    {
        private readonly Action<object, object, ValidationContext> _action;

        public DelegateConstraint(Action action)
            : this((_, __, ___) => action())
        {
        }

        public DelegateConstraint(Action<object, object, ValidationContext> func)
        {
            _action = func;
        }

        public void Validate(object instance, object value, ValidationContext context) => _action(instance, value, context);
    }
}
