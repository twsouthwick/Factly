// Copyright (c) Taylor Southwick. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;

namespace Factly
{
    /// <summary>
    /// Contains context information for validation that is passed through for validation.
    /// </summary>
    public sealed class ValidationContext<TState>
#if NO_CANCELLATION_TOKEN
        : ICancellable
#endif
    {
#if FEATURE_PARALLEL
        private const int DefaultMaxDegreeOfParallelism = 1;
#endif

        private static readonly Action<ValidationError> DefaultOnErrorHandler = error => throw new ValidationException(error);
        private static readonly Action<Type> DefaultOnUnknownTypeHandler = type => throw new ValidatorBuilderException(SR.UnknownTypeEncountered, Errors.UnknownType, type, null);
        private static readonly Action<object> DefaultOnItemHandler = _ => { };

        private readonly bool _isReadonly;

        private Action<ValidationError> _onError;
        private Action<Type> _onUnknownType;
        private Action<object> _onItem;

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

        internal ValidationContext(ValidationContext<TState> context, PropertyInfo property = null, IConstraint<TState> constraint = null, object instance = null)
        {
#if NO_CANCELLATION_TOKEN
            _other = context;
#endif
            _isReadonly = true;

            if (context != null)
            {
                _onError = context.OnError;
                _onItem = context.OnItem;
                _onUnknownType = context.OnUnknownType;

                State = context.State;
                Property = property ?? context.Property;
                Constraint = constraint ?? context.Constraint;
                Instance = instance ?? context.Instance;
            }
            else
            {
                Property = property;
                Constraint = constraint;
                Instance = instance;
            }

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
            get => _onUnknownType ?? DefaultOnUnknownTypeHandler;
            set
            {
                CheckIfReadonly();
                _onUnknownType = value;
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
            get => _onError ?? DefaultOnErrorHandler;
            set
            {
                CheckIfReadonly();
                _onError = value;
            }
        }

        /// <summary>
        /// Gets or sets the handler called when an item is processed.
        /// </summary>
        public Action<object> OnItem
        {
            get => _onItem ?? DefaultOnItemHandler;
            set
            {
                CheckIfReadonly();
                _onItem = value;
            }
        }

        internal PropertyInfo Property { get; }

        internal object Instance { get; }

        internal IConstraint<TState> Constraint { get; }

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
