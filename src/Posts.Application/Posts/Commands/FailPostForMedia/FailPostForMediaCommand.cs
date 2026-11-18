using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.FailPostForMedia;

public sealed record FailPostForMediaCommand(Guid MediaAssetId) : ICommand<Result>;
