namespace FC.Codeflix.Catalog.Application.Exceptions;
public abstract class ApplicationException : Exception
{
    protected ApplicationException(string? message) : base(message)
    {}
}
