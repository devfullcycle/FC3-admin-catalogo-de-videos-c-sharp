namespace FC.Codeflix.Catalog.Domain.Validation;
public class NotificationValidationHandler : ValidationHandler
{
    private readonly List<ValidationError> _errors;
    
    public NotificationValidationHandler()
        => _errors = new List<ValidationError>();
    
    public IReadOnlyCollection<ValidationError> Errors 
        => _errors.AsReadOnly();

    public bool HasErrors() => _errors.Count > 0;

    public override void HandleError(ValidationError error)
        => _errors.Add(error);
}
