using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedKernal.Results;
using Storage.Application.Files.Commands.UploadFile;
using Storage.Presentation.Files;

namespace Storage.Presentation.UnitTests;

public sealed class FilesControllerTests
{
    private readonly Mock<ISender> _senderMock;
    private readonly FilesController _controller;

    public FilesControllerTests()
    {
        _senderMock = new Mock<ISender>();
        _controller = new FilesController(_senderMock.Object);
    }

    [Fact]
    public async Task Upload_WithValidFile_ReturnsCreatedWithLocation()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var file = CreateFormFile("document.txt", "text/plain", [0x01, 0x02, 0x03]);

        _senderMock
            .Setup(s => s.Send(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(fileId));

        // Act
        var result = await _controller.Upload(file, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        Assert.Equal($"/api/files/{fileId}", createdResult.Location);

        var value = createdResult.Value!;
        var idProperty = value.GetType().GetProperty("id");
        Assert.NotNull(idProperty);
        Assert.Equal(fileId, idProperty.GetValue(value));
    }

    [Fact]
    public async Task Upload_WithEmptyFile_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateFormFile("empty.txt", "text/plain", []);
        var error = Error.Validation("File.Empty", "File cannot be empty.");

        _senderMock
            .Setup(s => s.Send(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(error));

        // Act
        var result = await _controller.Upload(file, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task Upload_WithTooLargeFile_ReturnsBadRequest()
    {
        // Arrange
        var file = CreateFormFile("big.bin", "application/octet-stream", []);
        var error = Error.Validation("File.TooLarge", "File size exceeds 100 MB limit.");

        _senderMock
            .Setup(s => s.Send(It.IsAny<UploadFileCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Guid>(error));

        // Act
        var result = await _controller.Upload(file, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    private static IFormFile CreateFormFile(string fileName, string contentType, byte[] content)
    {
        var stream = new MemoryStream(content);
        var fileMock = new Mock<IFormFile>();

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.Length).Returns(stream.Length);

        return fileMock.Object;
    }
}
