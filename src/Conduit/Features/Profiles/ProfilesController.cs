using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Conduit.Features.Profiles;

[Route("profiles")]
public class ProfilesController : Controller
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{username}")]
    public Task<ProfileEnvelope> Get(string username, CancellationToken cancellationToken) =>
        _mediator.Send(new Details.Query(username), cancellationToken);
}
