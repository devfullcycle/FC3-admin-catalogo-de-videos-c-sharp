using FC.Codeflix.Catalog.Api.ApiModels.Response;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.Common;
using FC.Codeflix.Catalog.Application.UseCases.CastMember.GetCastMember;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FC.Codeflix.Catalog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CastMembersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CastMembersController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CastMemberModelOutput>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var output = await _mediator.Send(new GetCastMemberInput(id), cancellationToken);
        return Ok(new ApiResponse<CastMemberModelOutput>(output));
    }
}
