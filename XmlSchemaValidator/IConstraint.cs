namespace XmlSchemaValidator
{
    internal interface IConstraint
    {
        bool Validate(object obj);
    }
}
