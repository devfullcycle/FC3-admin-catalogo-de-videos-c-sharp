namespace FC.Codeflix.Catalog.Domain.Validation;
public abstract class ValidationHandler
{
    public abstract void HandleError(ValidationError error);

    public void HandleError(string message)
        => HandleError(new ValidationError(message));
}
