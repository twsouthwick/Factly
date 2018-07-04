using System;

namespace ObjectValidator
{
    public sealed class ValidationContext
    {
        private static Action<ValidationError> DefaultErrorHandler = error => throw new ValidationException(error);
        private static Action<Type> DefaultUnknownTypeHandler = type => throw new ValidatorException("Unknown type", Errors.UnknownType, type, null);
        private static Action<object> DefaultItemHandler = _ => { };

        private readonly bool _isReadonly;
        private Action<ValidationError> _errors;
        private Action<Type> _unknownTypes;
        private Action<object> _items;

        public ValidationContext()
        {
            _isReadonly = false;
        }

        internal ValidationContext(ValidationContext context)
        {
#if NO_CANCELLATION_TOKEN
            _other = context;
#endif
            _isReadonly = true;

            _errors = context?.OnError ?? DefaultErrorHandler;
            _items = context?.OnItem ?? DefaultItemHandler;
            _unknownTypes = context?.OnUnknownType ?? DefaultUnknownTypeHandler;
        }

        public Action<Type> OnUnknownType
        {
            get => _unknownTypes;
            set
            {
                CheckIfReadonly();
                _unknownTypes = value;
            }
        }

        public Action<ValidationError> OnError
        {
            get => _errors;
            set
            {
                CheckIfReadonly();
                _errors = value;
            }
        }

        public Action<object> OnItem
        {
            get => _items;
            set
            {
                CheckIfReadonly();
                _items = value;
            }
        }

        private void CheckIfReadonly()
        {
            if (_isReadonly)
            {
                throw new InvalidOperationException(SR.ValidationStartedContextReadonly);
            }
        }

#if NO_CANCELLATION_TOKEN
        private readonly ValidationContext _other;
        private bool _isCancelled = false;

        public bool IsCancelled => _isCancelled || _other?.IsCancelled == true;

        public void Cancel() => _isCancelled = true;
#endif
    }
}
