using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Rule;
using CashRegisterAPI.Tests.TestData;
using CashRegisterAPI.Utility;
using Moq;
using DomainRule = CashRegisterAPI.Domain.Rule;

namespace CashRegisterAPI.Tests.Utility;

[TestFixture]
public class RuleEngineTests
{
    private static readonly BasicRuleInfoDTO AnyInfo = RuleInfo.Create(0, 9);

    private static Mock<IRule<string, BasicRuleInfoDTO>> MockRule(
        string name, bool isApplicable, string applyResult)
    {
        var mock = new Mock<IRule<string, BasicRuleInfoDTO>>();
        mock.Setup(r => r.Name()).Returns(name);
        mock.Setup(r => r.IsApplicable(It.IsAny<BasicRuleInfoDTO>())).Returns(isApplicable);
        mock.Setup(r => r.Apply(It.IsAny<BasicRuleInfoDTO>())).Returns(applyResult);
        return mock;
    }

    private static Mock<IRuleRepository> MockRepo(params DomainRule[] activeRules)
    {
        var mock = new Mock<IRuleRepository>();
        mock.Setup(r => r.GetActiveRules()).ReturnsAsync([.. activeRules]);
        return mock;
    }

    // Rule selection

    [Test]
    public async Task Start_AppliesNonDefaultRule_WhenItIsApplicable()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: true, applyResult: "3 pennies");
        var repo = MockRepo(
            new DomainRule(1, "minChange", 0, true),
            new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object], repo.Object, TestOptions.Default);

        var result = await engine.Start(AnyInfo);

        Assert.That(result, Is.EqualTo("3 pennies"));
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Once);
        minChange.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }

    [Test]
    public async Task Start_FallsBackToDefaultRule_WhenNoNonDefaultRuleIsApplicable()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: false, applyResult: "3 pennies");
        var repo = MockRepo(
            new DomainRule(1, "minChange", 0, true),
            new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object], repo.Object, TestOptions.Default);

        var result = await engine.Start(AnyInfo);

        Assert.That(result, Is.EqualTo("1 dollar"));
        minChange.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Once);
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }

    // Active rule filtering

    [Test]
    public async Task Start_IgnoresNonDefaultRule_WhenNotInActiveRules()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");
        var divisibleBy = MockRule("divisibleBy", isApplicable: true, applyResult: "3 pennies");

        // divisibleBy registered but not active in DB
        var repo = MockRepo(new DomainRule(1, "minChange", 0, true));

        var engine = new RuleEngine(
            [minChange.Object, divisibleBy.Object], repo.Object, TestOptions.Default);

        var result = await engine.Start(AnyInfo);

        Assert.That(result, Is.EqualTo("1 dollar"));
        divisibleBy.Verify(r => r.Apply(It.IsAny<BasicRuleInfoDTO>()), Times.Never);
    }

    // Error handling

    [Test]
    public void Start_ThrowsInvalidOperationException_WhenDefaultRuleNotActive()
    {
        var minChange = MockRule("minChange", isApplicable: true, applyResult: "1 dollar");

        // Active rules do not include "minChange"
        var repo = MockRepo(new DomainRule(2, "divisibleBy", 1, true));

        var engine = new RuleEngine(
            [minChange.Object], repo.Object, TestOptions.Default);

        Assert.ThrowsAsync<InvalidOperationException>(() => engine.Start(AnyInfo));
    }
}
