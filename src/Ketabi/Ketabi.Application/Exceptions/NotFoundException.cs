namespace Ketabi.Application.Exceptions;

public sealed class NotFoundException : ApplicationExceptionBase
{
    public NotFoundException(string message) : base(message)
    {
    }
}
