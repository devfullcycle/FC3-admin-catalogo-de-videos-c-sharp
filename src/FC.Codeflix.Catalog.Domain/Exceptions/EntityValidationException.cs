using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Exceptions;

public class EntityValidationException : Exception
{
    public IReadOnlyCollection<ValidationError>? Errors { get; }
    public EntityValidationException(
        string? message, 
        IReadOnlyCollection<ValidationError>? errors = null
    ) : base(message) 
        => Errors = errors;
}
