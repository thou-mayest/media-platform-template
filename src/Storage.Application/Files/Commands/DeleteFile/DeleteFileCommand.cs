using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Storage.Application.Files.Commands.DeleteFile;

internal sealed record DeleteFileCommand(Guid Id) : ICommand<Result<bool>>;
