using CashRegisterAPI.Domain;
using CashRegisterAPI.DTO;
using Xunit;
using CashRegisterAPI.Repository;
using CashRegisterAPI.Utility;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace CashRegisterAPI.Tests.Utility;

public class FileParserTests
{
    // Builds a Country entity with a single USD currency (separator '.') and two denominations.
    private static Country BuildCountry(int countryId = 1, int currencyId = 1)
    {
        var denominations = new List<Denomination>();
        var currency = new Currency(currencyId, "USD", '.', denominations, []);
        denominations.Add(new Denomination(1, "one dollar", null, 100, currency));
        denominations.Add(new Denomination(2, "penny", "pennies", 1, currency));

        var countryCurrencies = new List<CountryCurrency>();
        var country = new Country(countryId, "United States of America", "USA", 100, countryCurrencies);
        countryCurrencies.Add(new CountryCurrency(1, country, currency, true));

        return country;
    }

    private static IFormFile CreateFormFile(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.OpenReadStream()).Returns(stream);
        return mock.Object;
    }

    private static (FileParser parser, Mock<IRuleEngine> ruleEngineMock) BuildParser(
        Country country, int countryId = 1)
    {
        var repoMock = new Mock<ICountryRepository>();
        repoMock.Setup(r => r.GetById(countryId)).ReturnsAsync(country);

        var ruleEngineMock = new Mock<IRuleEngine>();
        ruleEngineMock
            .Setup(e => e.Start(It.IsAny<BasicRuleInfoDTO>()))
            .ReturnsAsync("1 dollar");

        var parser = new FileParser(repoMock.Object, ruleEngineMock.Object);
        return (parser, ruleEngineMock);
    }

    [Fact]
    public async Task ProcessFile_CallsRuleEngine_OncePerNonEmptyLine()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, engineMock) = BuildParser(country);

        var file = CreateFormFile("2.13,3.00\n1.97,2.00\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        await parser.ProcessFile(file, uploadInfo);

        engineMock.Verify(e => e.Start(It.IsAny<BasicRuleInfoDTO>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessFile_SkipsBlankLines()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, engineMock) = BuildParser(country);

        var file = CreateFormFile("2.13,3.00\n\n1.97,2.00\n\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        await parser.ProcessFile(file, uploadInfo);

        engineMock.Verify(e => e.Start(It.IsAny<BasicRuleInfoDTO>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessFile_ReturnsRuleEngineResultsAsUtf8Bytes()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var repoMock = new Mock<ICountryRepository>();
        repoMock.Setup(r => r.GetById(1)).ReturnsAsync(country);

        var engineMock = new Mock<IRuleEngine>();
        engineMock
            .SetupSequence(e => e.Start(It.IsAny<BasicRuleInfoDTO>()))
            .ReturnsAsync("3 pennies")
            .ReturnsAsync("1 dollar");

        var parser = new FileParser(repoMock.Object, engineMock.Object);
        var file = CreateFormFile("2.97,3.00\n1.00,2.00\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        var result = await parser.ProcessFile(file, uploadInfo);
        var text = Encoding.UTF8.GetString(result);

        Assert.Contains("3 pennies", text);
        Assert.Contains("1 dollar", text);
    }

    [Fact]
    public async Task ProcessFile_ThrowsFormatException_WhenLineHasInvalidFormat()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, _) = BuildParser(country);

        var file = CreateFormFile("invalid line\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        await Assert.ThrowsAsync<FormatException>(() => parser.ProcessFile(file, uploadInfo));
    }

    [Fact]
    public async Task ProcessFile_ThrowsFormatException_WhenLineHasOnlyOneAmount()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, _) = BuildParser(country);

        var file = CreateFormFile("2.00\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        await Assert.ThrowsAsync<FormatException>(() => parser.ProcessFile(file, uploadInfo));
    }

    [Fact]
    public async Task ProcessFile_ThrowsArgumentException_WhenAmountPaidLessThanAmountOwed()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, _) = BuildParser(country);

        // Paid (2.00) < Owed (3.00)
        var file = CreateFormFile("3.00,2.00\n");
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 1 };

        await Assert.ThrowsAsync<ArgumentException>(() => parser.ProcessFile(file, uploadInfo));
    }

    [Fact]
    public async Task ProcessFile_ThrowsArgumentException_WhenCurrencyNotAssociatedWithCountry()
    {
        var country = BuildCountry(countryId: 1, currencyId: 1);
        var (parser, _) = BuildParser(country);

        var file = CreateFormFile("2.00,3.00\n");
        // Request a currency ID that doesn't belong to this country
        var uploadInfo = new UploadInfoDto { CountryId = 1, CurrencyId = 999 };

        await Assert.ThrowsAsync<ArgumentException>(() => parser.ProcessFile(file, uploadInfo));
    }
}
