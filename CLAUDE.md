# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ASP.NET Core 10 REST API that calculates cash register change. Core logic: given an amount owed and amount paid, return the minimum denominations needed. Special rule: if the change amount is divisible by 3, denominations are randomly allocated (but the total must still be correct).

## Commands

**API** — run from `api/CashRegisterAPI/`:

```bash
dotnet build                        # Build
dotnet run                          # Run (HTTP: localhost:5158, HTTPS: localhost:7165)
dotnet run --launch-profile https   # Run with HTTPS profile
dotnet ef migrations add <Name>     # Add EF Core migration
dotnet ef database update           # Apply migrations
```

Swagger UI is available at `http://localhost:5158/swagger` when running.

**Tests** — run from `api/CashRegisterAPI.Tests/`:

```bash
dotnet test                              # Run all tests
dotnet test --filter "Name~MinChange"    # Run a subset by name
```

## Architecture

**Request flow:** `FileUploadController` → `FileParser` → `RuleEngine` → `IRule` implementations → repository layer → PostgreSQL

### Key Layers

- **Controllers/** — Single controller (`FileUploadController`) with `POST /fileupload`. Accepts a file (IFormFile) + `uploadInfo` (CountryId, CurrencyId) as form data. Returns a text file of change denominations.
- **Utility/** — `FileParser` reads/validates the file and drives rule execution per line. `RuleEngine` runs registered rules in priority order, applying the first applicable one.
- **Rule/** — Rule engine implementations behind `IRule<T,V>`. Two rules exist: `MinChangeRule` (greedy minimum denominations, priority 0) and `DivisibleByRule` (random allocation when change % divisor == 0, priority 1). Add new rules by implementing `IRule` and registering in `Program.cs`.
- **Repository/** — Generic repository pattern over EF Core. `CountryRepository` eager-loads currencies and denominations.
- **Data/** — `CashRegisterDbContext` + `DataSeeder` which seeds USA, France, Finland with USD/EURO denominations on first run.

### Rule Engine Extension Points

The README calls out three extensibility questions — these map directly to the code:
1. **Change the divisor** → `RuleEngineInfo:DivisibleBy:Divisor` in `appsettings.json`
2. **Add a new special case** → Implement `IRule<BasicRuleInfoDTO, string>`, register in DI, add a row to the `Rule` table
3. **New country (France)** → Already seeded; add to `DataSeeder.cs` or insert directly into DB

### Database

PostgreSQL on `localhost:5432`, database `CashRegister`, user `postgres` / password `password`. Schema is managed via EF Core migrations. The DB is auto-created and seeded on startup via `DataSeeder.SeedAsync()`.

Tables: `Country`, `Currency`, `Denomination`, `CountryCurrency` (junction), `Rule`.

Currency amounts are stored as integers using a multiplier (e.g., USA = 100, so $1.00 → 100 units).

### Configuration (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CashRegister;Username=postgres;Password=password"
  },
  "RuleEngineInfo": {
    "DefaultRuleName": "minChange",
    "DivisibleBy": { "Divisor": 3 }
  }
}
```

`DefaultRuleName` is the fallback used when no rule's `IsApplicable()` returns `true`. Each rule can define its own config block under `RuleEngineInfo` (e.g. `DivisibleBy` for `DivisibleByRule`).

CORS is configured to allow `http://localhost:3000` (expected React frontend).

## Tests

`api/CashRegisterAPI.Tests/` — NUnit 4 + Moq. No database required; all dependencies are mocked.

```
Controllers/    FileUploadController — response types and exception-to-status-code mapping
Rules/          MinChangeRule and DivisibleByRule — Apply/IsApplicable behaviour
Utility/        RuleEngine (rule selection/fallback) and FileParser (file parsing, validation)
TestData/       Shared readonly fixtures used across all test classes
```

**TestData classes** — prefer these over inline test data when adding new tests:

- `Denominations` — static readonly `DenominationDTO` fields and arrays (`AllCoins`, `AllUsd`)
- `RuleInfo` — `RuleInfo.Create(owed, paid, ...denominations)` factory (`currencyMultiplier=100`)
- `TestOptions` — `TestOptions.Default` / `TestOptions.WithDivisor(n)` for `IOptions<RuleEngineInfo>`
- `Countries` — `Countries.BuildUsa()` / `Countries.BuildFrance()` domain entity factories; uses the list-reference trick to resolve the circular `Currency ↔ Denomination` constructor dependency
- `FormFiles` — `FormFiles.Create(string)` for mock `IFormFile` instances
- `OutputParser` — `OutputParser.ParseTotal(output, denominations)` for verifying random-allocation rule output
