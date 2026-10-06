using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Profiles.Application.ActorProfiles.Commands.DeleteActorProfile;
using Profiles.Application.ActorProfiles.Commands.PublishActorProfile;
using Profiles.Application.ActorProfiles.Commands.UnpublishActorProfile;
using Profiles.Application.ActorProfiles.Queries.GetActorProfileBySlug;
using Profiles.Application.ActorProfiles.Queries.GetIndexableActorProfiles;
using Profiles.Application.ActorProfiles.Queries.GetMyActorProfile;
using Profiles.Presentation.Authorization;
using SharedKernal.Extensions;
using SharedKernal.Pagination;

namespace Profiles.Presentation.Profiles;

[ApiController]
[Route("api/profiles")]
public sealed class ProfilesController(ISender sender) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken ct)
    {
        var result = await sender.Send(
            new GetIndexableActorProfilesQuery(PageRequest.Create(page, pageSize)), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMine(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();

        var result = await sender.Send(new GetMyActorProfileQuery(userId), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<IActionResult> UpdateMine(
        [FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();

        var result = await sender.Send(request.ToCommand(userId), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }

    [HttpPost("me/publish")]
    [Authorize]
    public async Task<IActionResult> PublishMine(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();

        var result = await sender.Send(new PublishActorProfileCommand(userId), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }

    [HttpPost("me/unpublish")]
    [Authorize]
    public async Task<IActionResult> UnpublishMine(CancellationToken ct)
    {
        if (CurrentUserId is not Guid userId) return Unauthorized();

        var result = await sender.Send(new UnpublishActorProfileCommand(userId), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }

   
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct)
    {
        var result = await sender.Send(new GetActorProfileBySlugQuery(slug), ct);

        return result.Match(p => Ok(p.ToResponse()));
    }


    [HttpDelete("{id:guid}")]
    [Authorize(Policy = ProfilesPolicies.RequireAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteActorProfileCommand(id), ct);

        return result.Match(_ => (IActionResult)NoContent());
    }


    private Guid? CurrentUserId =>
        Guid.TryParse(User.FindFirst("sub")?.Value, out var id) ? id : null;
}