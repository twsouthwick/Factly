using System.ComponentModel.DataAnnotations;

namespace ObjectValidator
{
    public static class DataAnnotationExtensions
    {
        public static ValidatorBuilder AddRegexConstraint(this ValidatorBuilder builder)
        {
            return builder
                .AddRegexConstraint<RegularExpressionAttribute>(pattern => pattern.Pattern);
        }
    }
}
