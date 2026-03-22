using CashRegisterAPI.Controllers;
using CashRegisterAPI.Domain;
using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CashRegisterAPI.Tests.Controllers;

[TestFixture]
public class RuleControllerTests
{
    private Mock<IRuleRepository> _repoMock = null!;
    private RuleController _controller = null!;

    private static Domain.Rule MinChange => new(1, "minChange", 0, true);
    private static Domain.Rule DivisibleBy => new(2, "divisibleBy", 1, true);
    private static Domain.Rule InactiveRule => new(3, "legacy", 2, false);

    [SetUp]
    public void SetUp()
    {
        _repoMock = new Mock<IRuleRepository>();
        _controller = new RuleController(_repoMock.Object);
    }

    // GetAll

    [Test]
    public async Task GetAll_ReturnsOk_WithMappedDTOs()
    {
        _repoMock.Setup(r => r.GetAll()).ReturnsAsync([MinChange, DivisibleBy, InactiveRule]);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.StatusCode, Is.EqualTo(200));
        var dtos = (ok.Value as IEnumerable<RuleDTO>)!.ToList();
        Assert.That(dtos, Has.Count.EqualTo(3));
        Assert.That(dtos[0].Name, Is.EqualTo("minChange"));
        Assert.That(dtos[2].IsActive, Is.False);
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

    // GetActiveRules

    [Test]
    public async Task GetActiveRules_ReturnsOk_WithOnlyActiveRules()
    {
        _repoMock.Setup(r => r.GetActiveRules()).ReturnsAsync([MinChange, DivisibleBy]);

        var result = await _controller.GetActiveRules();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dtos = (ok.Value as IEnumerable<RuleDTO>)!.ToList();
        Assert.That(dtos, Has.Count.EqualTo(2));
        Assert.That(dtos.All(d => d.IsActive), Is.True);
    }

    [Test]
    public async Task GetActiveRules_Returns500_WhenExceptionThrown()
    {
        _repoMock.Setup(r => r.GetActiveRules()).ThrowsAsync(new Exception("db down"));

        var result = await _controller.GetActiveRules();

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }

    // GetById

    [Test]
    public async Task GetById_ReturnsOk_WithMappedDTO()
    {
        _repoMock.Setup(r => r.GetById(1)).ReturnsAsync(MinChange);

        var result = await _controller.GetById(1);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as RuleDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("minChange"));
        Assert.That(dto.Priority, Is.EqualTo(0));
        Assert.That(dto.IsActive, Is.True);
    }

    [Test]
    public async Task GetById_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetById(99)).ThrowsAsync(new InvalidOperationException("No rules exist with an id of: $99"));

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
        _repoMock.Setup(r => r.GetByName("minChange")).ReturnsAsync(MinChange);

        var result = await _controller.GetByName("minChange");

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var dto = ok!.Value as RuleDTO;
        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Name, Is.EqualTo("minChange"));
    }

    [Test]
    public async Task GetByName_ReturnsNotFound_WhenInvalidOperationExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("unknown")).ThrowsAsync(new InvalidOperationException("No rules exist with a name of: $unknown"));

        var result = await _controller.GetByName("unknown");

        var notFound = result as NotFoundObjectResult;
        Assert.That(notFound, Is.Not.Null);
        Assert.That(notFound!.StatusCode, Is.EqualTo(404));
    }

    [Test]
    public async Task GetByName_Returns500_WhenUnexpectedExceptionThrown()
    {
        _repoMock.Setup(r => r.GetByName("minChange")).ThrowsAsync(new Exception("unexpected"));

        var result = await _controller.GetByName("minChange");

        var obj = result as ObjectResult;
        Assert.That(obj, Is.Not.Null);
        Assert.That(obj!.StatusCode, Is.EqualTo(500));
    }
}
