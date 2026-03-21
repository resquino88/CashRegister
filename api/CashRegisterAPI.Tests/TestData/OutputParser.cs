using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Parses rule engine output strings back to a denomination total for verification.
/// Expected format: "N name, N name, ..." where name may be singular, plural (via PluralName),
/// or the denomination name with an appended 's'.
/// </summary>
public static class OutputParser
{
    public static long ParseTotal(string output, DenominationDTO[] denominations)
    {
        if (string.IsNullOrWhiteSpace(output)) return 0;
        long total = 0;
        foreach (var part in output.Split(", "))
        {
            var space = part.IndexOf(' ');
            var count = long.Parse(part[..space]);
            var name = part[(space + 1)..];
            var denom = denominations.First(d =>
                d.Name == name || d.PluralName == name || d.Name + "s" == name);
            total += count * denom.Value;
        }
        return total;
    }
}
