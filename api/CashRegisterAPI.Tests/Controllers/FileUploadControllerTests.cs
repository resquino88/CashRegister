using CashRegisterAPI.Controllers;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CashRegisterAPI.Tests.Controllers;

[TestFixture]
public class FileUploadControllerTests
{
    private Mock<IFileParser> _parserMock = null!;
    private FileUploadController _controller = null!;
    private IFormFile _anyFile = null!;
    private UploadInfoDto _anyUploadInfo = null!;

    [SetUp]
    public void SetUp()
    {
        _parserMock = new Mock<IFileParser>();
        _controller = new FileUploadController(_parserMock.Object);
        _anyFile = new Mock<IFormFile>().Object;
        _anyUploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };
    }

    // Success

    [Test]
    public async Task FileUpload_ReturnsFileResult_WithCorrectContentType_WhenSuccessful()
    {
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ReturnsAsync("3 pennies"u8.ToArray());

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo);

        var fileResult = result as FileContentResult;
        Assert.That(fileResult, Is.Not.Null);
        Assert.That(fileResult!.ContentType, Is.EqualTo("text/plain"));
        Assert.That(fileResult.FileDownloadName, Is.EqualTo("results.txt"));
    }

    [Test]
    public async Task FileUpload_ReturnsByteArrayFromParser()
    {
        var expected = "1 dollar"u8.ToArray();
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ReturnsAsync(expected);

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo) as FileContentResult;

        Assert.That(result!.FileContents, Is.EqualTo(expected));
    }

    // ArgumentException → 400

    [Test]
    public async Task FileUpload_ReturnsBadRequest_WhenArgumentExceptionThrown()
    {
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ThrowsAsync(new ArgumentException("Amount paid is less than amount owed."));

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.That(badRequest!.StatusCode, Is.EqualTo(400));
    }

    // FormatException → 400

    [Test]
    public async Task FileUpload_ReturnsBadRequest_WhenFormatExceptionThrown()
    {
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ThrowsAsync(new FormatException("Line is invalid."));

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo);

        var badRequest = result as BadRequestObjectResult;
        Assert.That(badRequest, Is.Not.Null);
        Assert.That(badRequest!.StatusCode, Is.EqualTo(400));
    }

    // InvalidOperationException → 500

    [Test]
    public async Task FileUpload_Returns500_WhenInvalidOperationExceptionThrown()
    {
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ThrowsAsync(new InvalidOperationException("No active default rule found."));

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
    }

    // Unhandled Exception → 500

    [Test]
    public async Task FileUpload_Returns500_WhenUnexpectedExceptionThrown()
    {
        _parserMock
            .Setup(p => p.ProcessFile(_anyFile, _anyUploadInfo))
            .ThrowsAsync(new Exception("Unexpected failure."));

        var result = await _controller.FileUpload(_anyFile, _anyUploadInfo);

        var objectResult = result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);
        Assert.That(objectResult!.StatusCode, Is.EqualTo(500));
    }
}
