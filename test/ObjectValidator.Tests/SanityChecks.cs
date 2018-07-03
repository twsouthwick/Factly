using Xunit;

namespace ObjectValidator
{
    public class SanityChecks
    {
        [Fact]
        public void ThrowsOnUnknown()
        {
            var validator = ValidatorBuilder.Create()
                .AddKnownType<int>()
                .Build();
            var exception = Assert.Throws<ValidatorException>(() => validator.Validate(string.Empty));

            Assert.Equal(typeof(string), exception.Type);
            Assert.Equal(Errors.UnknownType, exception.Id);
            Assert.Null(exception.Property);
        }
    }
}
