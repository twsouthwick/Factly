namespace ObjectValidator
{
    public interface IConstraint
    {
        void Validate(object instance, object value, ValidationContext context);
    }
}
