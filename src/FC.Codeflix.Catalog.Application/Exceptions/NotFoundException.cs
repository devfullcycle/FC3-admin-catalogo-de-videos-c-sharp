namespace FC.Codeflix.Catalog.Application.Exceptions;
public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message)
    {}

    public static void ThrowIfNull(
        object? @object, 
        string exceptionMessage)
    {
        if (@object == null)
            throw new NotFoundException(exceptionMessage);
    }
}
