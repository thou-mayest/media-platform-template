using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Storage.Application.Files.Commands.CompleteMediaUpload;
using Storage.Application.Files.Commands.FailMediaUpload;

namespace Storage.Presentation.Files;

[ApiController]
[AllowAnonymous]
[Route("api/files/webhooks/s3")]
public sealed class S3NotificationsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Handle([FromBody] S3NotificationRequest request, CancellationToken ct)
    {
        foreach (var record in request.Records ?? [])
        {
            var storageKey = Uri.UnescapeDataString(record.S3.Object.Key);

            if (record.EventName.StartsWith("ObjectCreated", StringComparison.OrdinalIgnoreCase))
            {
                await sender.Send(new CompleteMediaUploadCommand(storageKey), ct);
            }
            else if (record.EventName.StartsWith("ObjectRemoved", StringComparison.OrdinalIgnoreCase)
                || record.EventName.Equals("UploadFailed", StringComparison.OrdinalIgnoreCase))
            {
                await sender.Send(new FailMediaUploadCommand(storageKey), ct);
            }
        }

        return Ok();
    }
}
