import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import LandingPage from "./LandingPage";
import {
  mockCountries,
  mockUsaCountry,
  mockUsdCurrency,
} from "@/test/fixtures";

vi.mock("@/endpoints", () => ({
  getAllCountries: vi.fn(),
  uploadFile: vi.fn(),
  getAxiosError: vi.fn(),
}));

vi.mock("@/utils", () => ({
  downloadBlob: vi.fn(),
}));

vi.mock("react-hot-toast", () => ({
  default: {
    loading: vi.fn(() => "toast-id"),
    success: vi.fn(),
    error: vi.fn(),
  },
}));

vi.mock("@/pages", () => ({
  LoadingScreen: () => <div data-testid="loading-screen">Loading...</div>,
  ErrorScreen: ({
    message,
    onRetry,
  }: {
    message: string;
    onRetry: () => void;
  }) => (
    <div data-testid="error-screen">
      <span>{message}</span>
      <button onClick={onRetry}>Retry</button>
    </div>
  ),
}));

vi.mock("@/components", () => ({
  Header: ({ onToggleMode }: { onToggleMode: () => void }) => (
    <button data-testid="toggle-mode" onClick={onToggleMode}>
      Toggle
    </button>
  ),
  UploadInfo: () => <div data-testid="upload-info" />,
  UploadArea: ({
    onFileUpload,
  }: {
    onFileUpload: (f: File) => void;
    file: File | undefined;
    setFile: (f: File | undefined) => void;
    selectedCurrency: unknown;
  }) => (
    <button
      data-testid="trigger-upload"
      onClick={() => onFileUpload(new File([""], "test.txt"))}
    >
      Upload
    </button>
  ),
}));

import { getAllCountries, uploadFile, getAxiosError } from "@/endpoints";
import { downloadBlob } from "@/utils";
import toast from "react-hot-toast";

const mockGetAllCountries = vi.mocked(getAllCountries);
const mockUploadFile = vi.mocked(uploadFile);
const mockGetAxiosError = vi.mocked(getAxiosError);
const mockDownloadBlob = vi.mocked(downloadBlob);

beforeEach(() => {
  vi.clearAllMocks();
  mockGetAxiosError.mockResolvedValue("Something went wrong.");
});

describe("LandingPage", () => {
  describe("loading state", () => {
    it("renders LoadingScreen while getAllCountries is pending", () => {
      mockGetAllCountries.mockReturnValue(new Promise(() => {}));
      render(<LandingPage onToggleMode={vi.fn()} />);
      expect(screen.getByTestId("loading-screen")).toBeInTheDocument();
    });
  });

  describe("success state", () => {
    beforeEach(() => {
      mockGetAllCountries.mockResolvedValue(mockCountries);
    });

    it("renders Header, UploadInfo, and UploadArea after data loads", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => {
        expect(screen.getByTestId("toggle-mode")).toBeInTheDocument();
        expect(screen.getByTestId("upload-info")).toBeInTheDocument();
        expect(screen.getByTestId("trigger-upload")).toBeInTheDocument();
      });
    });

    it("does not render LoadingScreen after data loads", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(screen.queryByTestId("loading-screen")).not.toBeInTheDocument(),
      );
    });

    it("does not render ErrorScreen on successful load", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(screen.queryByTestId("error-screen")).not.toBeInTheDocument(),
      );
    });
  });

  describe("error state", () => {
    beforeEach(() => {
      mockGetAllCountries.mockRejectedValue(new Error("Network error"));
      mockGetAxiosError.mockResolvedValue("Network error");
    });

    it("renders ErrorScreen when getAllCountries rejects", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(screen.getByTestId("error-screen")).toBeInTheDocument(),
      );
    });

    it("passes the error message to ErrorScreen", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(screen.getByText("Network error")).toBeInTheDocument(),
      );
    });

    it("calls toast.error with the error message on failure", async () => {
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(toast.error).toHaveBeenCalledWith("Network error"),
      );
    });
  });

  describe("retry behaviour", () => {
    it("re-calls getAllCountries when retry is clicked", async () => {
      mockGetAllCountries
        .mockRejectedValueOnce(new Error("fail"))
        .mockResolvedValueOnce(mockCountries);

      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() =>
        expect(screen.getByTestId("error-screen")).toBeInTheDocument(),
      );
      await userEvent.click(screen.getByRole("button", { name: /retry/i }));
      await waitFor(() => expect(mockGetAllCountries).toHaveBeenCalledTimes(2));
    });

    it("shows success view after retry resolves", async () => {
      mockGetAllCountries
        .mockRejectedValueOnce(new Error("fail"))
        .mockResolvedValueOnce(mockCountries);

      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("error-screen"));
      await userEvent.click(screen.getByRole("button", { name: /retry/i }));
      await waitFor(() =>
        expect(screen.getByTestId("upload-info")).toBeInTheDocument(),
      );
    });
  });

  describe("initiateFileUpload", () => {
    beforeEach(() => {
      mockGetAllCountries.mockResolvedValue(mockCountries);
    });

    it("calls toast.loading when upload starts", async () => {
      mockUploadFile.mockResolvedValue(new Blob(["result"]));
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("trigger-upload"));
      await userEvent.click(screen.getByTestId("trigger-upload"));
      expect(toast.loading).toHaveBeenCalledWith("Processing file...");
    });

    it("calls uploadFile with countryId and currencyId", async () => {
      mockUploadFile.mockResolvedValue(new Blob(["result"]));
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("trigger-upload"));
      await userEvent.click(screen.getByTestId("trigger-upload"));
      await waitFor(() =>
        expect(mockUploadFile).toHaveBeenCalledWith(expect.any(File), {
          countryId: mockUsaCountry.id,
          currencyId: mockUsdCurrency.id,
        }),
      );
    });

    it("calls downloadBlob with the result on success", async () => {
      const blob = new Blob(["result"]);
      mockUploadFile.mockResolvedValue(blob);
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("trigger-upload"));
      await userEvent.click(screen.getByTestId("trigger-upload"));
      await waitFor(() =>
        expect(mockDownloadBlob).toHaveBeenCalledWith(blob, "results.txt"),
      );
    });

    it("calls toast.success on successful upload", async () => {
      mockUploadFile.mockResolvedValue(new Blob(["result"]));
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("trigger-upload"));
      await userEvent.click(screen.getByTestId("trigger-upload"));
      await waitFor(() =>
        expect(toast.success).toHaveBeenCalledWith(
          "Results downloaded successfully",
          { id: "toast-id" },
        ),
      );
    });

    it("calls toast.error on upload failure", async () => {
      mockUploadFile.mockRejectedValue(new Error("Upload failed"));
      mockGetAxiosError.mockResolvedValue("Upload failed");
      render(<LandingPage onToggleMode={vi.fn()} />);
      await waitFor(() => screen.getByTestId("trigger-upload"));
      await userEvent.click(screen.getByTestId("trigger-upload"));
      await waitFor(() =>
        expect(toast.error).toHaveBeenCalledWith("Upload failed", {
          id: "toast-id",
        }),
      );
    });
  });

  describe("onToggleMode", () => {
    it("passes onToggleMode through to Header", async () => {
      mockGetAllCountries.mockResolvedValue(mockCountries);
      const onToggleMode = vi.fn();
      render(<LandingPage onToggleMode={onToggleMode} />);
      await waitFor(() => screen.getByTestId("toggle-mode"));
      await userEvent.click(screen.getByTestId("toggle-mode"));
      expect(onToggleMode).toHaveBeenCalledTimes(1);
    });
  });
});
