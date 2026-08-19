using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Queries.GetActorProfileBySlug;

internal sealed record GetActorProfileBySlugQuery(string Slug) : IQuery<Result<ActorProfileDto>>;