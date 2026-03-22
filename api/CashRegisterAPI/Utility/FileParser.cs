using CashRegisterAPI.DTO;
using CashRegisterAPI.Repository;
using System.Text;
using System.Text.RegularExpressions;

namespace CashRegisterAPI.Utility;

public class FileParser(ICountryRepository countryRepository, IRuleEngine ruleEngine) : IFileParser
{
    public async Task<byte[]> ProcessFile(IFormFile file, UploadInfoDto uploadInfo)
    {
        var country = CountryDTO.FromEntity(await countryRepository.GetById(uploadInfo.CountryId));
        var currency = country.Currencies.SingleOrDefault(c => c.Id == uploadInfo.CurrencyId);

        if(currency == null)
        {
            throw new ArgumentException($"Currency with id '{uploadInfo.CurrencyId}' is not associated with country id '{uploadInfo.CountryId}'.");
        }

        System.Globalization.NumberFormatInfo info = new()
        {
            NumberDecimalSeparator = currency.CurrencySeparator + ""
        };

        string pattern = $@"[0-9]+{Regex.Escape(currency.CurrencySeparator.ToString())}[0-9]+";

        var lines = new List<string>();

        using (var stream = file.OpenReadStream())
        using (var reader = new StreamReader(stream))
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    lines.Add(line.Trim());
            }
        }

        var results = new List<string>();
        MatchCollection matches;
        long amountOwed;
        long amountPaid;

        foreach(var line in lines)
        {
            matches = Regex.Matches(line, pattern);

            if(matches.Count != 2)
            {
                throw new FormatException($"Line '{line}' is invalid. Expected format: amount{currency.CurrencySeparator}amount (e.g. 2{currency.CurrencySeparator}13,3{currency.CurrencySeparator}00).");
            }

            amountOwed = (long)(decimal.Parse(matches[0].Value, info) * country.CurrencyMultiplier);
            amountPaid = (long)(decimal.Parse(matches[1].Value, info) * country.CurrencyMultiplier);

            if(amountOwed > amountPaid)
            {
                throw new ArgumentException($"Amount paid ({amountPaid}) is less than amount owed ({amountOwed}) on line '{line}'.");
            }

            var ruleInfo = new BasicRuleInfoDTO(amountOwed, amountPaid, country.CurrencyMultiplier, [.. currency.Denominations]);

            var result = await ruleEngine.Start(ruleInfo);
            results.Add(result);
        }

        return Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, results));
    }
}
