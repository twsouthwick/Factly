using System;

namespace ObjectValidator
{
    public class ValidationContext
    {
        public Action<ValidationError> Errors { get; set; }

        public Action<object> Items { get; set; }
    }
}
