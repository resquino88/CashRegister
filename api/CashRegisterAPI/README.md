# CashRegister API

ASP.NET Core 10 REST API that calculates cash register change denominations from an uploaded file.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL running on `localhost:5432`

## NuGet Packages

These are restored automatically via `dotnet run` / `dotnet build`, but listed here for reference:

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.EntityFrameworkCore` | 10.0.5 | ORM |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.5 | EF Core CLI tools (migrations) |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.1 | PostgreSQL provider for EF Core |
| `Swashbuckle.AspNetCore` | 6.4.0 | Swagger / OpenAPI UI |

To restore manually:

```bash
dotnet restore
```

## Setup

1. Ensure PostgreSQL is running with a user `postgres` / password `password` (or update `appsettings.json`).
2. The database and schema are created automatically on first run — no manual migration step needed.

```bash
cd api/CashRegisterAPI
dotnet run
```

API: `http://localhost:5158`
Swagger UI: `http://localhost:5158/swagger`

## Configuration

`appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=CashRegister;Username=postgres;Password=password"
  },
  "RuleEngineInfo": {
    "DefaultRuleName": "minChange",
    "DivisibleBy": {
      "Divisor": 3
    }
  }
}
```

`DefaultRuleName` is the fallback rule used when no other rule's `IsApplicable()` returns `true`. `DivisibleBy` is an example of a rule-specific config block — each new rule can define its own variables under `RuleEngineInfo` in the same way.

## Endpoints

### `POST /fileupload`

Accepts a file and returns a plain-text file with the change denominations for each line.

**Form fields:**

| Field | Type | Description |
|-------|------|-------------|
| `file` | file | Each line: `amountOwed,amountPaid` (e.g. `2.13,3.00`) |
| `CountryId` | int | ID of the country (see seeded data below) |
| `CurrencyId` | int | ID of the currency associated with that country |

**Response:** `results.txt` (text/plain) — one line of change denominations per input line.

**Example input file:**
```
2.12,3.00
1.97,2.00
3.33,5.00
```

**Example output:**
```
3 quarters,1 dime,3 pennies
3 pennies
1 dollar,1 quarter,6 nickels,12 pennies
```

> If the change amount is divisible by 3, denominations are randomly assigned — the total is still correct, but distribution varies each run.

**Error responses:**

| Status | Cause |
|--------|-------|
| `400` | Invalid file format, amount paid < amount owed, or mismatched country/currency |
| `500` | Unexpected server error |

---

### `GET /country`

Returns all countries.

### `GET /country/{id}`

Returns a country by numeric ID. Returns `404` if not found.

### `GET /country/{name}`

Returns a country by name. Returns `404` if not found.

---

### `GET /currency`

Returns all currencies.

### `GET /currency/{id}`

Returns a currency by numeric ID. Returns `404` if not found.

### `GET /currency/{name}`

Returns a currency by name. Returns `404` if not found.

---

### `GET /denomination`

Returns all denominations.

### `GET /denomination/{id}`

Returns a denomination by numeric ID. Returns `404` if not found.

### `GET /denomination/{name}`

Returns a denomination by name. Returns `404` if not found.

---

### `GET /rule`

Returns all rules (active and inactive).

### `GET /rule/active`

Returns only rules where `IsActive` is `true`.

### `GET /rule/{id}`

Returns a rule by numeric ID. Returns `404` if not found.

### `GET /rule/{name}`

Returns a rule by name. Returns `404` if not found.

---

All read endpoints return `500` on unexpected errors.

## Seeded Data

On first startup the database is populated with:

| Country | Currency | Decimal Separator |
|---------|----------|-------------------|
| United States of America (USA) | USD | `.` |
| France (FRA) | EURO | `,` |

**USD denominations (10):** penny, nickel, dime, quarter, one dollar, five dollar, ten dollar, twenty dollar, fifty dollar, one hundred dollar

**EURO denominations (15):** one cent, two cent, five cent, ten cent, twenty cent, fifty cent, one euro, two euro, five euro, ten euro, twenty euro, fifty euro, one hundred euro, two hundred euro, five hundred euro

**Rules seeded:**

| Name | Priority | Active |
|------|----------|--------|
| `minChange` | 0 | Yes |
| `divisibleBy` | 1 | Yes |

## Adding a New Rule

1. Implement `IRule<BasicRuleInfoDTO, string>` in the `Rule/` folder.
2. Register it in `Program.cs` via DI.
3. Insert a row into the `Rule` table with a unique name and priority.

Rules are executed in ascending priority order. The first rule where `IsApplicable()` returns `true` is applied; remaining rules are skipped.

## Tests

Tests live in `api/CashRegisterAPI.Tests/` and use NUnit 4 with Moq.

```bash
cd api/CashRegisterAPI.Tests
dotnet test                              # Run all tests
dotnet test --filter "Name~MinChange"   # Run a subset by name
```

**Test packages:**

| Package | Version | Purpose |
|---------|---------|---------|
| `NUnit` | 4.3.2 | Test framework |
| `NUnit3TestAdapter` | 5.0.0 | VS / CLI test runner adapter |
| `Moq` | 4.20.72 | Mocking dependencies |
| `Microsoft.NET.Test.Sdk` | 17.14.0 | Test host |
| `coverlet.collector` | 6.0.4 | Code coverage |

**Structure:**

```
CashRegisterAPI.Tests/
├── Controllers/        FileUploadController, CountryController, CurrencyController,
│                       DenominationController, RuleController — response types and error handling
├── Rules/              MinChangeRule and DivisibleByRule — apply/isApplicable logic
├── Utility/            RuleEngine and FileParser — orchestration and file parsing
└── TestData/           Shared readonly data used across all test classes
```

**TestData — shared fixtures:**

| File | Provides |
|------|----------|
| `Denominations.cs` | Static readonly `DenominationDTO` fields (Penny, Nickel, Dime, Quarter, Dollar, bills) and arrays `AllCoins`, `AllUsd` |
| `RuleInfo.cs` | `RuleInfo.Create(owed, paid, ...denominations)` — `BasicRuleInfoDTO` factory with `currencyMultiplier=100` |
| `TestOptions.cs` | `TestOptions.Default` + `TestOptions.WithDivisor(n)` — shared `IOptions<RuleEngineInfo>` |
| `Countries.cs` | `Countries.BuildUsa()` / `Countries.BuildFrance()` — domain entity factories |
| `FormFiles.cs` | `FormFiles.Create(string)` — mock `IFormFile` backed by a UTF-8 `MemoryStream` |
| `OutputParser.cs` | `OutputParser.ParseTotal(output, denominations)` — parses rule output strings back to a total for verifying random allocation |
