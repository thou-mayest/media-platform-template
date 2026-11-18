using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.PublishPostForMedia;

public sealed record PublishPostForMediaCommand(Guid MediaAssetId) : ICommand<Result>;
