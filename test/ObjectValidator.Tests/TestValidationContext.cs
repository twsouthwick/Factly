using System;
using System.Collections.Generic;

namespace ObjectValidator
{
    public class TestValidationContext
    {
        public TestValidationContext()
        {
            Errors = new List<ValidationError>();
            Items = new List<object>();
            UnknownTypes = new List<Type>();
            Context = new ValidationContext
            {
                Errors = Errors.Add,
                Items = Items.Add,
                UnknownType = UnknownTypes.Add
            };
        }

        public List<ValidationError> Errors { get; }

        public List<object> Items { get; }

        public List<Type> UnknownTypes { get; }

        public ValidationContext Context { get; }
    }
}
