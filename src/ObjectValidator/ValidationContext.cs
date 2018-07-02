﻿using System;

namespace ObjectValidator
{
    public class ValidationContext
    {
        public IObserver<ValidationError> Errors { get; set; }

        public IObserver<object> Items { get; set; }

        internal void OnCompleted()
        {
            Errors?.OnCompleted();
            Items?.OnCompleted();
        }
    }
}
