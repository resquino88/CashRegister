import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import Header from "./Header";
import {
  mockCountries,
  mockUsaCountry,
  mockFranceCountry,
  mockUsdCurrency,
  mockEuroCurrency,
  mockTheme,
} from "@/test/fixtures";

vi.mock("@mui/material", async () => {
  const actual = await vi.importActual("@mui/material");
  return { ...(actual as object), useTheme: () => mockTheme };
});

const mockSetHeaderData = vi.fn();
const mockOnToggleMode = vi.fn();

const defaultProps = {
  countries: mockCountries,
  headerData: undefined,
  setHeaderData: mockSetHeaderData,
  onToggleMode: mockOnToggleMode,
};

beforeEach(() => {
  vi.clearAllMocks();
});

describe("Header", () => {
  describe("rendering", () => {
    it('renders the "Cash Register" brand text', () => {
      render(<Header {...defaultProps} />);
      expect(screen.getByText("Cash Register")).toBeInTheDocument();
    });

    it("renders a menu item for each country", async () => {
      render(<Header {...defaultProps} />);
      const [countrySelect] = screen.getAllByRole("combobox");
      await userEvent.click(countrySelect);
      expect(
        await screen.findByText("United States (USA)"),
      ).toBeInTheDocument();
      expect(await screen.findByText("France (FR)")).toBeInTheDocument();
    });

    it("renders the dark mode switch", () => {
      render(<Header {...defaultProps} />);
      expect(screen.getByRole("switch")).toBeInTheDocument();
    });

    it("switch is unchecked in light mode", () => {
      render(<Header {...defaultProps} />);
      expect(screen.getByRole("switch")).not.toBeChecked();
    });

    it("currency select is disabled when headerData is undefined", () => {
      render(<Header {...defaultProps} headerData={undefined} />);
      const selects = screen.getAllByRole("combobox");
      expect(selects[1]).toHaveAttribute("aria-disabled", "true");
    });
  });

  describe("country selection", () => {
    it("calls setHeaderData with the selected country and its first currency", async () => {
      render(<Header {...defaultProps} />);
      const [countrySelect] = screen.getAllByRole("combobox");
      await userEvent.click(countrySelect);
      await userEvent.click(await screen.findByText("United States (USA)"));
      expect(mockSetHeaderData).toHaveBeenCalledWith({
        selectedCountry: mockUsaCountry,
        selectedCurrency: mockUsdCurrency,
      });
    });

    it("auto-selects the first currency of the chosen country", async () => {
      render(<Header {...defaultProps} />);
      const [countrySelect] = screen.getAllByRole("combobox");
      await userEvent.click(countrySelect);
      await userEvent.click(await screen.findByText("France (FR)"));
      expect(mockSetHeaderData).toHaveBeenCalledWith({
        selectedCountry: mockFranceCountry,
        selectedCurrency: mockEuroCurrency,
      });
    });
  });

  describe("currency selection", () => {
    it("calls setHeaderData when a currency is selected", async () => {
      render(
        <Header
          {...defaultProps}
          headerData={{
            selectedCountry: mockUsaCountry,
            selectedCurrency: undefined,
          }}
        />,
      );
      const selects = screen.getAllByRole("combobox");
      await userEvent.click(selects[1]);
      await userEvent.click(await screen.findByText("USD"));
      expect(mockSetHeaderData).toHaveBeenCalled();
    });

    it("does nothing if headerData is undefined", () => {
      render(<Header {...defaultProps} headerData={undefined} />);
      expect(mockSetHeaderData).not.toHaveBeenCalled();
    });
  });

  describe("theme toggle", () => {
    it("calls onToggleMode when the switch is clicked", async () => {
      render(<Header {...defaultProps} />);
      await userEvent.click(screen.getByRole("switch"));
      expect(mockOnToggleMode).toHaveBeenCalledTimes(1);
    });
  });
});
