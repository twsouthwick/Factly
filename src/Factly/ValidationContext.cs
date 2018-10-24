// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Factly
{
    /// <summary>
    /// Contains context information for validation that is passed through for validation.
    /// </summary>
#if NO_CANCELLATION_TOKEN
    public sealed class ValidationContext<TState> : ICancellable
#else
    public sealed class ValidationContext<TState>
#endif
    {
#if FEATURE_PARALLEL
        private const int DefaultMaxDegreeOfParallelism = 1;
#endif

        private static readonly Action<ValidationError> DefaultErrorHandler = error => throw new ValidationException(error);
        private static readonly Action<Type> DefaultUnknownTypeHandler = type => throw new ValidatorBuilderException(SR.UnknownTypeEncountered, Errors.UnknownType, type, null);
        private static readonly Action<object> DefaultItemHandler = _ => { };

        private readonly bool _isReadonly;

        private Action<ValidationError> _errors;
        private Action<Type> _unknownTypes;
        private Action<object> _items;

#if FEATURE_PARALLEL
        private int _maxDegreeOfParallelism = DefaultMaxDegreeOfParallelism;
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationContext{TState}"/> class.
        /// </summary>
        public ValidationContext(TState state = default)
        {
            _isReadonly = false;
            State = state;
        }

        internal ValidationContext(ValidationContext<TState> context)
        {
#if NO_CANCELLATION_TOKEN
            _other = context;
#endif
            _isReadonly = true;

            _errors = context?.OnError ?? DefaultErrorHandler;
            _items = context?.OnItem ?? DefaultItemHandler;
            _unknownTypes = context?.OnUnknownType ?? DefaultUnknownTypeHandler;
            State = context == null ? default : context.State;

#if FEATURE_PARALLEL
            _maxDegreeOfParallelism = context?.MaxDegreeOfParallelism ?? MaxDegreeOfParallelism;
#endif
        }

        /// <summary>
        /// Gets the state associated with this validation context.
        /// </summary>
        public TState State { get; }

        /// <summary>
        /// Gets or sets the handler called when an unknown type is encountered.
        /// </summary>
        public Action<Type> OnUnknownType
        {
            get => _unknownTypes;
            set
            {
                CheckIfReadonly();
                _unknownTypes = value;
            }
        }

#if FEATURE_PARALLEL
        /// <summary>
        /// Gets or sets the number of threads when used for parallel validation.
        /// </summary>
        public int MaxDegreeOfParallelism
        {
            get => _maxDegreeOfParallelism;
            set
            {
                CheckIfReadonly();
                _maxDegreeOfParallelism = value;
            }
        }
#endif

        /// <summary>
        /// Gets or sets the handler called when an error occurs.
        /// </summary>
        public Action<ValidationError> OnError
        {
            get => _errors;
            set
            {
                CheckIfReadonly();
                _errors = value;
            }
        }

        /// <summary>
        /// Gets or sets the handler called when an item is processed.
        /// </summary>
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

#pragma warning disable SA1201 // Elements should appear in the correct order
#if NO_CANCELLATION_TOKEN
        private readonly ValidationContext<TState> _other;
        private bool _isCancelled = false;

        /// <summary>
        /// Gets a value indicating whether the validation has been cancelled.
        /// </summary>
        public bool IsCancelled => _isCancelled || _other?.IsCancelled == true;

        /// <summary>
        /// Cancels the validation.
        /// </summary>
        public void Cancel() => _isCancelled = true;
#endif
#pragma warning restore SA1201 // Elements should appear in the correct order
    }
}
