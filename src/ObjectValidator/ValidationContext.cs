using System;

namespace ObjectValidator
{
    public class ValidationContext
    {
        public Action<ValidationError> Errors { get; set; }

        public Action<object> Items { get; set; }

#if !FEATURE_CANCELLATION_TOKEN
        internal bool IsCancelled { get; private set; }

        public void Cancel() => IsCancelled = true;
#endif
    }
}
