using System;

namespace ObjectValidator
{
    internal class ActionConstraint : IConstraint
    {
        private readonly Action _action;

        public ActionConstraint(Action action)
        {
            _action = action;
        }

        public ValidationError Validate(object instance, object value)
        {
            _action();
            return null;
        }
    }

    internal class DelegateConstraint : IConstraint
    {
        private readonly Func<object, object, ValidationError> _func;

        public DelegateConstraint(Action<object> action)
            : this((_, value) => action(value))
        {
        }

        public DelegateConstraint(Action<object, object> action)
            : this((instance, value) =>
            {
                action(instance, value);
                return null;
            })
        {
        }

        public DelegateConstraint(Func<object, object, ValidationError> func)
        {
            _func = func;
        }

        public ValidationError Validate(object instance, object value) => _func(instance, value);
    }
}
