namespace Chronos.Shared.Exceptions;

public class TokenMissingValueException : Exception
{
    public string ValueName { get; }

    public TokenMissingValueException(string valueName)
        : base($"Token is missing required value: {valueName}")
    {
        ValueName = valueName;
    }

    public TokenMissingValueException(string valueName, Exception innerException)
        : base($"Token is missing required value: {valueName}", innerException)
    {
        ValueName = valueName;
    }
}
