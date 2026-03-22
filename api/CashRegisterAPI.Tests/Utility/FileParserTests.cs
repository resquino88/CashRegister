using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Tests.TestData;
using CashRegisterAPI.Utility;
using Moq;
using System.Text;

namespace CashRegisterAPI.Tests.Utility;

[TestFixture]
public class FileParserTests
{
    private static (FileParser parser, Mock<IRuleEngine> engineMock) BuildParser(
        int countryId = 1)
    {
        var repoMock = new Mock<ICountryRepository>();
        repoMock.Setup(r => r.GetById(countryId)).ReturnsAsync(Countries.BuildUsa(countryId));

        var engineMock = new Mock<IRuleEngine>();
        engineMock
            .Setup(e => e.Start(It.IsAny<BasicRuleInfoDTO>()))
            .ReturnsAsync("1 dollar");

        return (new FileParser(repoMock.Object, engineMock.Object), engineMock);
    }

    // Rule engine invocation

    [Test]
    public async Task ProcessFile_CallsRuleEngine_OncePerNonEmptyLine()
    {
        var (parser, engineMock) = BuildParser();

        await parser.ProcessFile(
            FormFiles.Create("2.13,3.00\n1.97,2.00\n"),
            new UploadInfoDto { CountryId = 1, CurrencyId = 1 });

        engineMock.Verify(e => e.Start(It.IsAny<BasicRuleInfoDTO>()), Times.Exactly(2));
    }

    [Test]
    public async Task ProcessFile_SkipsBlankLines()
    {
        var (parser, engineMock) = BuildParser();

        await parser.ProcessFile(
            FormFiles.Create("2.13,3.00\n\n1.97,2.00\n\n"),
            new UploadInfoDto { CountryId = 1, CurrencyId = 1 });

        engineMock.Verify(e => e.Start(It.IsAny<BasicRuleInfoDTO>()), Times.Exactly(2));
    }

    // Output

    [Test]
    public async Task ProcessFile_ReturnsUtf8EncodedRuleEngineResults()
    {
        var repoMock = new Mock<ICountryRepository>();
        repoMock.Setup(r => r.GetById(1)).ReturnsAsync(Countries.BuildUsa());

        var engineMock = new Mock<IRuleEngine>();
        engineMock.SetupSequence(e => e.Start(It.IsAny<BasicRuleInfoDTO>()))
            .ReturnsAsync("3 pennies")
            .ReturnsAsync("1 dollar");

        var parser = new FileParser(repoMock.Object, engineMock.Object);

        var result = await parser.ProcessFile(
            FormFiles.Create("2.97,3.00\n1.00,2.00\n"),
            new UploadInfoDto { CountryId = 1, CurrencyId = 1 });

        var text = Encoding.UTF8.GetString(result);
        Assert.That(text, Does.Contain("3 pennies"));
        Assert.That(text, Does.Contain("1 dollar"));
    }

    // Zero change

    [Test]
    public async Task ProcessFile_ReturnsNoChange_WhenAmountOwedEqualsAmountPaid()
    {
        var repoMock = new Mock<ICountryRepository>();
        repoMock.Setup(r => r.GetById(1)).ReturnsAsync(Countries.BuildUsa());

        var engineMock = new Mock<IRuleEngine>();
        engineMock.Setup(e => e.Start(It.IsAny<BasicRuleInfoDTO>())).ReturnsAsync("No change");

        var parser = new FileParser(repoMock.Object, engineMock.Object);

        var result = await parser.ProcessFile(
            FormFiles.Create("0.00,0.00\n"),
            new UploadInfoDto { CountryId = 1, CurrencyId = 1 });

        Assert.That(System.Text.Encoding.UTF8.GetString(result), Is.EqualTo("No change"));
    }

    // Format validation

    [Test]
    public void ProcessFile_ThrowsFormatException_WhenLineContainsInvalidCharacters()
    {
        var (parser, _) = BuildParser();
        Assert.ThrowsAsync<FormatException>(() =>
            parser.ProcessFile(
                FormFiles.Create("2.00,abc\n"),
                new UploadInfoDto { CountryId = 1, CurrencyId = 1 }));
    }

    [Test]
    public void ProcessFile_ThrowsFormatException_WhenLineHasNoValidAmounts()
    {
        var (parser, _) = BuildParser();
        Assert.ThrowsAsync<FormatException>(() =>
            parser.ProcessFile(
                FormFiles.Create("invalid line\n"),
                new UploadInfoDto { CountryId = 1, CurrencyId = 1 }));
    }

    [Test]
    public void ProcessFile_ThrowsFormatException_WhenLineHasOnlyOneAmount()
    {
        var (parser, _) = BuildParser();
        Assert.ThrowsAsync<FormatException>(() =>
            parser.ProcessFile(
                FormFiles.Create("2.00\n"),
                new UploadInfoDto { CountryId = 1, CurrencyId = 1 }));
    }

    // Business rule validation

    [Test]
    public void ProcessFile_ThrowsArgumentException_WhenAmountPaidLessThanAmountOwed()
    {
        var (parser, _) = BuildParser();
        // Paid (2.00) < Owed (3.00)
        Assert.ThrowsAsync<ArgumentException>(() =>
            parser.ProcessFile(
                FormFiles.Create("3.00,2.00\n"),
                new UploadInfoDto { CountryId = 1, CurrencyId = 1 }));
    }

    [Test]
    public void ProcessFile_ThrowsArgumentException_WhenCurrencyNotAssociatedWithCountry()
    {
        var (parser, _) = BuildParser();
        Assert.ThrowsAsync<ArgumentException>(() =>
            parser.ProcessFile(
                FormFiles.Create("2.00,3.00\n"),
                new UploadInfoDto { CountryId = 1, CurrencyId = 999 }));
    }
}
