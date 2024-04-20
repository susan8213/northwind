namespace Northwind.Exceptions;


public abstract class ApplicationException : Exception
{
    public abstract string ErrorCode { get; }
    public ApplicationException() {}
    public ApplicationException(string message) : base(message) {}
}

public class NotFoundException : ApplicationException
{
    public override string ErrorCode => "NotFound";
    public NotFoundException() {}
    public NotFoundException(string message) : base(message) { }
}

public class ValidationException : ApplicationException
{
    public override string ErrorCode => "ValidationError";
    public ValidationException() {}
    public ValidationException(string message) : base(message) { }
}