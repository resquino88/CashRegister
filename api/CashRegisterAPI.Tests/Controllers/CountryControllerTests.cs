using CashRegisterAPI.Controllers;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CashRegisterAPI.Tests.Controllers;

[TestFixture]
public class CountryControllerTests
{
    private Mock<ICountryRepository> _repoMock = null!;
    private CountryController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<ICountryRepository>();
        _controller = new CountryController(_repoMock.Object);
    }

    // GetAll

    [Test]
    public async Task GetAll_ReturnsOk_WithMappedDTOs()
    {
        var usa = Countries.BuildUsa();
        var france = Countries.BuildFrance();
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync([usa, france]);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        var dtos = (ok.Value as IEnumerable<CountryDTO>)!.ToList();
        Assert.That(dtos, Has.Count.EqualTo(2));
        Assert.That(dtos[0].Name, Is.EqualTo("United States of America"));
        Assert.That(dtos[1].Name, Is.EqualTo("France"));
    }

    [Test]
    public async Task GetAll_Returns500_WhenExceptionThrown()
    {
        _repoMock.Setup(r => r.GetAll()).ThrowsAsync(new Exception("db down"));

        var result = await _controller.GetAll();

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }

    // GetById

    [Test]
    public async Task GetById_ReturnsOk_WithMappedDTO()
    {
        var usa = Countries.BuildUsa();
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(usa);

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as CountryDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("United States of America"));
        Assert.That(dto.Abbrevation, Is.EqualTo("USA"));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetById(99)).ThrowsAsync(new InvalidOperationException("No countries exist with an id of: $99"));

        var result = await _controller.GetById(99);

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetById_Returns500_WhenUnexpectedExceptionThrown()
    {
        _repoMock.Setup(r => r.GetById(1)).ThrowsAsync(new Exception("unexpected"));

        var result = await _controller.GetById(1);

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }

    // GetByName

    [Test]
    public async Task GetByName_ReturnsOk_WithMappedDTO()
    {
        var usa = Countries.BuildUsa();
        _repoMock.Setup(r => r.GetByName("United States of America")).ReturnsAsync(usa);

        var result = await _controller.GetByName("United States of America");

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as CountryDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Abbrevation, Is.EqualTo("USA"));
    }

    [Test]
    public async Task GetByName_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("Nowhere")).ThrowsAsync(new InvalidOperationException("No countries exist with a name of: $Nowhere"));

        var result = await _controller.GetByName("Nowhere");

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetByName_Returns500_WhenUnexpectedExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("USA")).ThrowsAsync(new Exception("unexpected"));

        var result = await _controller.GetByName("USA");

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }
}
