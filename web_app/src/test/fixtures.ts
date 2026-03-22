import type { Country, Currency } from "@/endpoints";

export const mockUsdCurrency: Currency = {
  id: 1,
  name: "USD",
  currencySeparator: ".",
  denominations: [],
};

export const mockEuroCurrency: Currency = {
  id: 2,
  name: "EUR",
  currencySeparator: ",",
  denominations: [],
};

export const mockUsaCountry: Country = {
  id: 1,
  name: "United States",
  abbrevation: "USA",
  currencyMultiplier: 100,
  currencies: [mockUsdCurrency],
};

export const mockFranceCountry: Country = {
  id: 2,
  name: "France",
  abbrevation: "FR",
  currencyMultiplier: 100,
  currencies: [mockEuroCurrency],
};

export const mockCountries: Country[] = [mockUsaCountry, mockFranceCountry];

export const mockTheme = {
  palette: {
    mode: "light" as const,
    background: { paper: "#fff", default: "#fafafa" },
    divider: "#e0e0e0",
    primary: { main: "#1976d2", light: "#42a5f5", dark: "#1565c0" },
    success: { main: "#2e7d32" },
    error: { main: "#c62828" },
    text: { primary: "#000", secondary: "#666", disabled: "#bbb" },
    action: { hover: "#f5f5f5" },
  },
};
