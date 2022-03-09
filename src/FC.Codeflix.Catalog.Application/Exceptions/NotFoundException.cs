namespace FC.Codeflix.Catalog.Application.Exceptions;
public class NotFoundException : ApplicationException
{
    public NotFoundException(string? message) : base(message)
    {}
}
