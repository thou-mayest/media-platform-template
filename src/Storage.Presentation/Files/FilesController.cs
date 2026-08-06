using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernal.Extensions;
using Storage.Application.Files.Commands.UploadFile;
using Storage.Application.Files.Queries.GetAllFiles;
using Storage.Application.Files.Queries.GetFileById;
using Storage.Presentation.Authorization;

namespace Storage.Presentation.Files;

[ApiController]
[Authorize]
[Route("api/files")]
public sealed class FilesController(ISender sender) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = StoragePolicies.RequireAdmin)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
    {
        var command = new UploadFileCommand(
            file.OpenReadStream(),
            file.FileName,
            file.ContentType,
            file.Length);

        var result = await sender.Send(command, ct);

        return result.Match(
            id => Created($"/api/files/{id}", new { id })
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = StoragePolicies.RequireAdmin)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetFileByIdQuery(id), ct);

        return result.Match(
            file => Ok(file)
        );
    }

    [HttpGet]
    [Authorize(Policy = StoragePolicies.RequireAdmin)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllFilesQuery(), ct);

        return result.Match(
            files => Ok(files)
        );
    }
}
