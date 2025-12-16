namespace Chronos.Shared.Exceptions;

public class UnexpectedErrorException : Exception
{
    public UnexpectedErrorException()
    {
    }

    public UnexpectedErrorException(string? message) : base(message)
    {
    }

    public UnexpectedErrorException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}