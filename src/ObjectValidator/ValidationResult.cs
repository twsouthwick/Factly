#if MISSING_FEATURE_IREADONLYCOLLECTION
using ValidationErrorCollection = System.Collections.Generic.IEnumerable<ObjectValidator.ValidationError>;
using ObjectCollection = System.Collections.Generic.IEnumerable<object>;
#else
using ValidationErrorCollection = System.Collections.Generic.IReadOnlyCollection<ObjectValidator.ValidationError>;
using ObjectCollection = System.Collections.Generic.IReadOnlyCollection<object>;
#endif

namespace ObjectValidator
{
    public class ValidationResult
    {
        internal ValidationResult(
            ValidationErrorCollection errors,
            ObjectCollection items
            )
        {
            Errors = errors;
            Items = items;
        }

        public ValidationErrorCollection Errors { get; }

        public ObjectCollection Items { get; }
    }
}
