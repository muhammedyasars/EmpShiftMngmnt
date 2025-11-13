namespace Domain;
 
public abstract class DomainException : Exception
{
    public string Code { get; }

    protected DomainException(string message, string code) : base(message)
    {
        Code = code;
    }
}
public class ValidationException : DomainException
{
    public ValidationException(string message, string code = "VALIDATION_ERROR") : base(message, code)
    {
    }
}
public class NotFoundException : DomainException
{
    public NotFoundException(string message, string code = "NOT_FOUND") : base(message, code)
    {
    }
}
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message, string code = "BUSINESS_RULE_VIOLATION") : base(message, code)
    {
    }
}