using System;

namespace ObjectValidator
{
    public sealed class ValidationContext
    {
        private static Action<ValidationError> DefaultErrorHandler = error => throw new ValidationException(error);
        private static Action<object> DefaultItemHandler = _ => { };

        private readonly bool _isReadonly;
        private Action<ValidationError> _errors;
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

            _errors = context?.Errors ?? DefaultErrorHandler;
            _items = context?.Items ?? DefaultItemHandler;
        }

        public Action<ValidationError> Errors
        {
            get => _errors;
            set
            {
                CheckIfReadonly();
                _errors = value;
            }
        }

        public Action<object> Items
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
                throw new InvalidOperationException("Context has been marked as readonly as validation has already started");
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
