import { render, screen } from "@testing-library/react";
import UploadArea from "./UploadArea";
import { mockUsdCurrency, mockTheme } from "@/test/fixtures";

let capturedOnDrop: ((files: File[]) => void) | undefined;
let capturedDisabled: boolean | undefined;

vi.mock("react-dropzone", () => ({
  useDropzone: (opts: { onDrop: (f: File[]) => void; disabled: boolean }) => {
    capturedOnDrop = opts.onDrop;
    capturedDisabled = opts.disabled;
    return {
      getRootProps: () => ({ "data-testid": "dropzone" }),
      getInputProps: () => ({}),
      isDragActive: false,
    };
  },
}));

vi.mock("@mui/material", async () => {
  const actual = await vi.importActual("@mui/material");
  return { ...(actual as object), useTheme: () => mockTheme };
});

const mockSetFile = vi.fn();
const mockOnFileUpload = vi.fn();

const defaultProps = {
  file: undefined,
  setFile: mockSetFile,
  selectedCurrency: mockUsdCurrency,
  onFileUpload: mockOnFileUpload,
};

beforeEach(() => {
  vi.clearAllMocks();
  capturedOnDrop = undefined;
  capturedDisabled = undefined;
});

describe("UploadArea", () => {
  describe("disabled state (no selectedCurrency)", () => {
    it('renders "No currency has been selected" when disabled', () => {
      render(<UploadArea {...defaultProps} selectedCurrency={undefined} />);
      expect(
        screen.getByText("No currency has been selected"),
      ).toBeInTheDocument();
    });

    it("passes disabled=true to useDropzone", () => {
      render(<UploadArea {...defaultProps} selectedCurrency={undefined} />);
      expect(capturedDisabled).toBe(true);
    });
  });

  describe("enabled state (selectedCurrency provided, no file)", () => {
    it("renders the drag & drop prompt text", () => {
      render(<UploadArea {...defaultProps} />);
      expect(
        screen.getByText("Drag & drop a file here, or click to browse"),
      ).toBeInTheDocument();
    });

    it("passes disabled=false to useDropzone", () => {
      render(<UploadArea {...defaultProps} />);
      expect(capturedDisabled).toBe(false);
    });
  });

  describe("file drop", () => {
    it("calls setFile with the dropped file", () => {
      render(<UploadArea {...defaultProps} />);
      const file = new File(["content"], "test.txt", { type: "text/plain" });
      capturedOnDrop?.([file]);
      expect(mockSetFile).toHaveBeenCalledWith(file);
    });

    it("does not call setFile when onDrop receives an empty array", () => {
      render(<UploadArea {...defaultProps} />);
      capturedOnDrop?.([]);
      expect(mockSetFile).not.toHaveBeenCalled();
    });
  });

  describe("file selected state", () => {
    it("renders the file name when file prop is set", () => {
      const file = new File([""], "myfile.txt");
      render(<UploadArea {...defaultProps} file={file} />);
      expect(screen.getByText("myfile.txt")).toBeInTheDocument();
    });
  });

  describe("useEffect — onFileUpload trigger", () => {
    it("calls onFileUpload when file prop changes from undefined to a File", () => {
      const file = new File([""], "test.txt");
      const { rerender } = render(
        <UploadArea {...defaultProps} file={undefined} />,
      );
      expect(mockOnFileUpload).not.toHaveBeenCalled();
      rerender(<UploadArea {...defaultProps} file={file} />);
      expect(mockOnFileUpload).toHaveBeenCalledWith(file);
    });

    it("does not call onFileUpload on initial render when file is undefined", () => {
      render(<UploadArea {...defaultProps} file={undefined} />);
      expect(mockOnFileUpload).not.toHaveBeenCalled();
    });

    it("calls onFileUpload again when the file prop changes to a different file", () => {
      const file1 = new File([""], "first.txt");
      const file2 = new File([""], "second.txt");
      const { rerender } = render(
        <UploadArea {...defaultProps} file={file1} />,
      );
      rerender(<UploadArea {...defaultProps} file={file2} />);
      expect(mockOnFileUpload).toHaveBeenCalledTimes(2);
      expect(mockOnFileUpload).toHaveBeenLastCalledWith(file2);
    });
  });
});
