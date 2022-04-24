namespace FC.Codeflix.Catalog.Application.Exceptions;
public class RelatedAggregateException : ApplicationException
{
    public RelatedAggregateException(string? message) : base(message)
    { }
}