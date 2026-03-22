using CashRegisterAPI.Controllers;
using CashRegisterAPI.Domain;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CashRegisterAPI.Tests.Controllers;

[TestFixture]
public class CurrencyControllerTests
{
    private Mock<ICurrencyRepository> _repoMock = null!;
    private CurrencyController _controller = null!;

    private static Currency BuildUsd()
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(1, "USD", '.', denominations, []);
        denominations.Add(new Denomination(1, "one dollar", null, 100, currency));
        denominations.Add(new Denomination(2, "penny", "pennies", 1, currency));
        return currency;
    }

    private static Currency BuildEuro()
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(2, "EURO", ',', denominations, []);
        denominations.Add(new Denomination(3, "one euro", null, 100, currency));
        denominations.Add(new Denomination(4, "one cent", null, 1, currency));
        return currency;
    }

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<ICurrencyRepository>();
        _controller = new CurrencyController(_repoMock.Object);
    }

    // GetAll

    [Test]
    public async Task GetAll_ReturnsOk_WithMappedDTOs()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync([BuildUsd(), BuildEuro()]);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        var dtos = (ok.Value as IEnumerable<CurrencyDTO>)!.ToList();
        Assert.That(dtos, Has.Count.EqualTo(2));
        Assert.That(dtos[0].Name, Is.EqualTo("USD"));
        Assert.That(dtos[1].Name, Is.EqualTo("EURO"));
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
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(BuildUsd());

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as CurrencyDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("USD"));
        Assert.That(dto.CurrencySeparator, Is.EqualTo('.'));
        Assert.That(dto.Denominations, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetById(99)).ThrowsAsync(new InvalidOperationException("No currencies exist with an id of: $99"));

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
        _repoMock.Setup(r => r.GetByName("USD")).ReturnsAsync(BuildUsd());

        var result = await _controller.GetByName("USD");

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as CurrencyDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("USD"));
    }

    [Test]
    public async Task GetByName_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("GBP")).ThrowsAsync(new InvalidOperationException("No currencies exist with a name of: $GBP"));

        var result = await _controller.GetByName("GBP");

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetByName_Returns500_WhenUnexpectedExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("USD")).ThrowsAsync(new Exception("unexpected"));

        var result = await _controller.GetByName("USD");

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }
}
