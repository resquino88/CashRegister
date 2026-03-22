using CashRegisterAPI.Domain;
using System.Xml.Linq;

namespace CashRegisterAPI.DTO;

public record CountryDTO(int Id, string Name, string Abbrevation, int CurrencyMultiplier, List<CurrencyDTO> Currencies)
{
    public static CountryDTO FromEntity(Country country) => new(country.Id, country.Name, country.Abbrevation, country.CurrencyMultipler, [.. country.CountryCurrencies.Select(cc => CurrencyDTO.FromEntity(cc.Currency))]);
}

