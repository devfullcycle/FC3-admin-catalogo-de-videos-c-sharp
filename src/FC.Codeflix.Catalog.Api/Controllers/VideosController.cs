using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Api.ApiModels.Video;
using FC.Codeflix.Catalog.Application.UseCases.Video.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class VideosController : ControllerBase
{
    private readonly IMediator _mediator;

    public VideosController(IMediator mediator)
        => _mediator = mediator;

    [HttpPost()]
    [ProducesResponseType(typeof(ApiResponse<VideoModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateVideo(
        [FromBody]CreateVideoApiInput request,
        CancellationToken cancellationToken)
    {
        var input = request.ToCreateVideoInput();
        var output = await _mediator.Send(input, cancellationToken);
        return CreatedAtAction(
            nameof(CreateVideo),
            new { id = output.Id },
            new ApiResponse<VideoModelOutput>(output));
    }
}
