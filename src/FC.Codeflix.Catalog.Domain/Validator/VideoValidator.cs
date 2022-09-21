using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.Validation;

namespace FC.Codeflix.Catalog.Domain.Validator;
public class VideoValidator : Validation.Validator
{
    private readonly Video _video;
    
    public VideoValidator(Video video, ValidationHandler handler) 
        : base(handler)
            => _video = video;

    public override void Validate()
    {
    }
}
