using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Storage.Application.Files.Commands.CompleteMediaUpload;

internal sealed record CompleteMediaUploadCommand(string StorageKey) : ICommand<Result>;
