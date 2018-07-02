namespace ObjectValidator
{
    public interface IConstraint
    {
        ValidationError Validate(object instance, object value);
    }
}
