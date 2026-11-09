using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Storage.Application.Files.Commands.FailMediaUpload;

internal sealed record FailMediaUploadCommand(string StorageKey) : ICommand<Result>;
