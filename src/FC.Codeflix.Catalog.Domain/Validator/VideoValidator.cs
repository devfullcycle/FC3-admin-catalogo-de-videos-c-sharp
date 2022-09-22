using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator;
public class VideoValidator : Validation.Validator
{
    private readonly Video _video;

    private const int TitleMaxLength = 255;
    private const int DescriptionMaxLength = 4_000;

    public VideoValidator(Video video, ValidationHandler handler) 
        : base(handler)
            => _video = video;

    public override void Validate()
    {
        ValidateTitle();

        if (string.IsNullOrWhiteSpace(_video.Description))
            _handler.HandleError($"'{nameof(_video.Description)}' is required");

        if(_video.Description.Length > DescriptionMaxLength)
            _handler.HandleError($"'{nameof(_video.Description)}' should be less or equal {DescriptionMaxLength} characters long");
    }

    private void ValidateTitle()
    {
        if (string.IsNullOrWhiteSpace(_video.Title))
            _handler.HandleError($"'{nameof(_video.Title)}' is required");

        if (_video.Title.Length > 255)
            _handler.HandleError($"'{nameof(_video.Title)}' should be less or equal {TitleMaxLength} characters long");
    }
}
