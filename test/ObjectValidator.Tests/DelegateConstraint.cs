using System;

namespace ObjectValidator
{
    internal class DelegateConstraint : IConstraint
        {
            private readonly Action _action;

            public DelegateConstraint(Action action)
            {
                _action = action;
            }

            public ValidationError Validate(object instance, object value)
            {
                _action();
                return null;
            }
            }
}
