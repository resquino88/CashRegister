# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Full-stack cash register change calculator. The **API** (`api/`) is an ASP.NET Core 10 REST API: given an amount owed and amount paid, it returns the minimum denominations needed. Special rule: if the owed amount is divisible by 3, denominations are randomly allocated (but the total must still be correct). The **frontend** (`web_app/`) is a React + TypeScript + Vite app that lets users upload a transaction file, select a country/currency, and download the calculated change as a text file.

## Commands

**API** — run from `api/CashRegisterAPI/`:

```bash
dotnet build                        # Build
dotnet run                          # Run (HTTP: localhost:5158, HTTPS: localhost:7165)
dotnet run --launch-profile https   # Run with HTTPS profile
dotnet ef migrations add <Name>     # Add EF Core migration
dotnet ef database update           # Apply migrations
dotnet format CashRegisterAPI.sln   # Format all C# files
```

Swagger UI is available at `http://localhost:5158/swagger` when running.

**API Tests** — run from `api/CashRegisterAPI.Tests/`:

```bash
dotnet test                              # Run all tests
dotnet test --filter "Name~MinChange"    # Run a subset by name
```

**Frontend** — run from `web_app/`:

```bash
npm install        # Install dependencies
npm run dev        # Start dev server at http://localhost:3000
npm run build      # Production build
npm run preview    # Preview production build
npm run lint       # Run ESLint
npm run format     # Run Prettier (writes in place, respects .gitignore)
npm run test       # Run Vitest once
npm run test:watch # Run Vitest in watch mode
```

## Architecture

### API

**Request flow:** `FileUploadController` → `FileParser` → `RuleEngine` → `IRule` implementations → repository layer → PostgreSQL

#### Key Layers

- **Controllers/** — Five controllers:
  - `FileUploadController` — `POST /fileupload`. Accepts a file (IFormFile) + `uploadInfo` (CountryId, CurrencyId) as form data. Returns a text file of change denominations.
  - `CountryController` — `GET /country`, `GET /country/{id}`, `GET /country/{name}`
  - `CurrencyController` — `GET /currency`, `GET /currency/{id}`, `GET /currency/{name}`
  - `DenominationController` — `GET /denomination`, `GET /denomination/{id}`, `GET /denomination/{name}`
  - `RuleController` — `GET /rule`, `GET /rule/active`, `GET /rule/{id}`, `GET /rule/{name}`
- **Utility/** — `FileParser` reads/validates the file and drives rule execution per line. `RuleEngine` runs registered rules in priority order, applying the first applicable one.
- **Rule/** — Rule engine implementations behind `IRule<T,V>`. Two rules exist: `MinChangeRule` (greedy minimum denominations, priority 0) and `DivisibleByRule` (random allocation when change % divisor == 0, priority 1). Add new rules by implementing `IRule` and registering in `Program.cs`.
- **Repository/** — Generic repository pattern over EF Core. `CountryRepository` eager-loads currencies and denominations.
- **Data/** — `CashRegisterDbContext` + `DataSeeder` which seeds USA and France with USD/EURO denominations on first run.

#### Rule Engine Extension Points

1. **Change the divisor** → `RuleEngineInfo:DivisibleBy:Divisor` in `appsettings.json`
2. **Add a new special case** → Implement `IRule<BasicRuleInfoDTO, string>`, register in DI, add a row to the `Rule` table
3. **Add a new country** → Add to `DataSeeder.cs` or insert directly into the DB

#### Database

PostgreSQL on `localhost:5432`, database `CashRegister`, user `postgres` / password `password`. Schema is managed via EF Core migrations. The DB is auto-created and seeded on startup via `DataSeeder.SeedAsync()`.

Tables: `Country`, `Currency`, `Denomination`, `CountryCurrency` (junction), `Rule`.

Currency amounts are stored as integers using a multiplier (e.g., USA = 100, so $1.00 → 100 units).

#### Seeded Data

| Country | Currency | Decimal Separator |
|---------|----------|-------------------|
| United States of America (USA) | USD | `.` |
| France (FRA) | EURO | `,` |

Rules seeded: `minChange` (priority 0), `divisibleBy` (priority 1).

#### Configuration (`appsettings.json`)

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

CORS is configured to allow `http://localhost:3000` (the React frontend dev server).

### Frontend

React 19 + TypeScript + Vite. MUI v7 component library with a token-based theming system (light/dark mode). Axios for HTTP with HTTP/HTTPS switching via `VITE_USE_HTTPS` env var.

#### Environment (`.env` in `web_app/`)

```env
VITE_API_URL=http://localhost:5158
VITE_API_URL_HTTPS=https://localhost:7165
VITE_USE_HTTPS=true
```

#### Key Folders

- `src/components/` — `Header`, `UploadArea`, `UploadInfo` (barrel: `@/components`)
- `src/pages/` — `LandingPage`, `LoadingScreen`, `ErrorScreen` (barrel: `@/pages`)
- `src/endpoints/` — Axios client + `getAllCountries()` / `uploadFile()` (barrel: `@/endpoints`)
- `src/utils/` — `downloadBlob()` (barrel: `@/utils`)
- `src/theme/` — `tokens.ts` (raw values) + `theme.ts` (`getTheme(mode)` factory)
- `src/test/` — `setup.ts` (jest-dom import) + `fixtures.ts` (shared mock data)

The `@/` path alias maps to `src/`.

## API Tests

`api/CashRegisterAPI.Tests/` — NUnit 4 + Moq. No database required; all dependencies are mocked.

```
Controllers/    FileUploadController, CountryController, CurrencyController,
                DenominationController, RuleController — response types and error handling
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

## Frontend Tests

`web_app/src/` — Vitest + React Testing Library + `@testing-library/jest-dom`. Tests live alongside the files they test (`.test.tsx`).

```
components/     Header, UploadArea, UploadInfo
pages/          LandingPage, LoadingScreen, ErrorScreen
```

MUI's `useTheme` is mocked via `vi.mock('@mui/material', async () => { const actual = await vi.importActual(...); return { ...actual, useTheme: () => mockTheme } })`. External modules (`@/endpoints`, `@/utils`, `react-hot-toast`) are fully mocked in `LandingPage.test.tsx`. Shared mock data lives in `src/test/fixtures.ts`.
