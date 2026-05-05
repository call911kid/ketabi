namespace Ketabi.Application.Exceptions;

public sealed class BadRequestException : ApplicationExceptionBase
{
    public BadRequestException(string message) : base(message)
    {
    }
}
