import { render, screen } from "@testing-library/react";
import UploadInfo from "./UploadInfo";
import { mockTheme } from "@/test/fixtures";

vi.mock("@mui/material", async () => {
  const actual = await vi.importActual("@mui/material");
  return { ...(actual as object), useTheme: () => mockTheme };
});

describe("UploadInfo", () => {
  describe("static content", () => {
    it('renders the "Guidelines" heading', () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(screen.getByText("Guidelines")).toBeInTheDocument();
    });

    it("renders the subtitle", () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(
        screen.getByText("Instructions for formatting your transaction file"),
      ).toBeInTheDocument();
    });

    it('renders the "File Format" section heading', () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(screen.getByText("File Format")).toBeInTheDocument();
    });

    it('renders the "Notes" section heading', () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(screen.getByText("Notes")).toBeInTheDocument();
    });

    it("renders all four notes", () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(
        screen.getByText(/Supports .txt, .csv, .tsv file formats/),
      ).toBeInTheDocument();
      expect(
        screen.getByText(/Each line produces one line of output/),
      ).toBeInTheDocument();
      expect(
        screen.getByText(
          /Select the appropriate country and currency before uploading/,
        ),
      ).toBeInTheDocument();
      expect(
        screen.getByText(/both amounts must always include decimals/),
      ).toBeInTheDocument();
    });
  });

  describe('with period separator (".")', () => {
    it("renders example lines with period separator", () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(screen.getByText("2.13,3.00")).toBeInTheDocument();
      expect(screen.getByText("1.97,2.00")).toBeInTheDocument();
      expect(screen.getByText("3.33,5.00")).toBeInTheDocument();
    });

    it('includes "3.00" in the decimals note', () => {
      render(<UploadInfo decimalSeparator="." />);
      expect(screen.getByText(/e\.g\. 3\.00/)).toBeInTheDocument();
    });
  });

  describe('with comma separator (",")', () => {
    it("renders example lines with comma separator", () => {
      render(<UploadInfo decimalSeparator="," />);
      expect(screen.getByText("2,13,3,00")).toBeInTheDocument();
      expect(screen.getByText("1,97,2,00")).toBeInTheDocument();
      expect(screen.getByText("3,33,5,00")).toBeInTheDocument();
    });

    it('includes "3,00" in the decimals note', () => {
      render(<UploadInfo decimalSeparator="," />);
      expect(screen.getByText(/e\.g\. 3,00/)).toBeInTheDocument();
    });
  });
});
