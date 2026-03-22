using CashRegisterAPI.Controllers;
using CashRegisterAPI.Domain;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Tests.TestData;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CashRegisterAPI.Tests.Controllers;

[TestFixture]
public class DenominationControllerTests
{
    private Mock<IDenominationRepository> _repoMock = null!;
    private DenominationController _controller = null!;

    private static Denomination BuildDenomination(int id, string name, string? pluralName, int value)
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(1, "USD", '.', denominations, []);
        var denomination = new Denomination(id, name, pluralName, value, currency);
        denominations.Add(denomination);
        return denomination;
    }

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IDenominationRepository>();
        _controller = new DenominationController(_repoMock.Object);
    }

    // GetAll

    [Test]
    public async Task GetAll_ReturnsOk_WithMappedDTOs()
    {
        var penny = BuildDenomination(1, "penny", "pennies", 1);
        var dollar = BuildDenomination(2, "one dollar", null, 100);
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync([penny, dollar]);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        var dtos = (ok.Value as IEnumerable<DenominationDTO>)!.ToList();
        Assert.That(dtos, Has.Count.EqualTo(2));
        Assert.That(dtos[0].Name, Is.EqualTo("penny"));
        Assert.That(dtos[1].Name, Is.EqualTo("one dollar"));
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
        var penny = BuildDenomination(1, "penny", "pennies", 1);
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(penny);

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as DenominationDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("penny"));
        Assert.That(dto.PluralName, Is.EqualTo("pennies"));
        Assert.That(dto.Value, Is.EqualTo(1));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetById(99)).ThrowsAsync(new InvalidOperationException("No denominations exist with an id of: $99"));

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
        var penny = BuildDenomination(1, "penny", "pennies", 1);
        _repoMock.Setup(r => r.GetByName("penny")).ReturnsAsync(penny);

        var result = await _controller.GetByName("penny");

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as DenominationDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("penny"));
    }

    [Test]
    public async Task GetByName_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("doubloon")).ThrowsAsync(new InvalidOperationException("No denominations exist with a name of: $doubloon"));

        var result = await _controller.GetByName("doubloon");

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetByName_Returns500_WhenUnexpectedExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("penny")).ThrowsAsync(new Exception("unexpected"));

        var result = await _controller.GetByName("penny");

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }
}
