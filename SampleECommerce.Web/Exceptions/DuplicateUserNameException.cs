namespace SampleECommerce.Web.Exceptions;

public class DuplicateUserNameException : Exception
{
    public DuplicateUserNameException()
    {
    }

    public DuplicateUserNameException(string message) : base(message)
    {
    }

    public DuplicateUserNameException(string duplicateUserName, string message, Exception innerException)
        : base(message, innerException)
    {
        DuplicateUserName = duplicateUserName;
    }
    
    public string? DuplicateUserName { get; }
}