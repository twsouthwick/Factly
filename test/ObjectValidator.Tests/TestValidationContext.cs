using System.Collections.Generic;

namespace ObjectValidator
{
    public class TestValidationContext
        {
            public TestValidationContext()
            {
                Errors = new List<ValidationError>();
                Items = new List<object>();
                Context = new ValidationContext
                {
                    Errors = Errors.Add,
                    Items = Items.Add
                };
            }

            public List<ValidationError> Errors { get; }

            public List<object> Items { get; }

            public ValidationContext Context { get; }
        }
}
