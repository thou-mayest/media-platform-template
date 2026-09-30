using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Posts.Application.Posts.Commands.RecordPostView;

public sealed record RecordPostViewCommand(Guid Id) : ICommand<Result>;
