using CashRegisterAPI.DTO;

namespace CashRegisterAPI.Tests.TestData;

/// <summary>
/// Shared readonly denomination DTOs. Values match the seeded USD/EURO data.
/// All denominations belong to currency id=1 for test purposes.
/// </summary>
public static class Denominations
{
    // USD coins
    public static readonly DenominationDTO Penny   = new(1, "penny",         "pennies", 1,     1);
    public static readonly DenominationDTO Nickel  = new(2, "nickel",        null,      5,     1);
    public static readonly DenominationDTO Dime    = new(3, "dime",          null,      10,    1);
    public static readonly DenominationDTO Quarter = new(4, "quarter",       null,      25,    1);

    // USD bills
    public static readonly DenominationDTO Dollar      = new(5, "one dollar",       null, 100,   1);
    public static readonly DenominationDTO FiveDollar  = new(6, "five dollar",      null, 500,   1);
    public static readonly DenominationDTO TenDollar   = new(7, "ten dollar",       null, 1000,  1);
    public static readonly DenominationDTO TwentyDollar = new(8, "twenty dollar",   null, 2000,  1);

    // Groupings
    public static readonly DenominationDTO[] AllCoins    = [Quarter, Dime, Nickel, Penny];
    public static readonly DenominationDTO[] AllUsd      = [TwentyDollar, TenDollar, FiveDollar, Dollar, Quarter, Dime, Nickel, Penny];
}
