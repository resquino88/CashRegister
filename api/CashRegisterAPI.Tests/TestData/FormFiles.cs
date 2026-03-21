using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Factory for mock IFormFile instances backed by in-memory UTF-8 streams.
/// </summary>
public static class FormFiles
{
    public static IFormFile Create(string content)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.OpenReadStream()).Returns(stream);
        return mock.Object;
    }
}
