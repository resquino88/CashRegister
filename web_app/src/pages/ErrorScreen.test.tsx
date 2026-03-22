import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import ErrorScreen from "./ErrorScreen";
import { mockTheme } from "@/test/fixtures";

vi.mock("@mui/material", async () => {
  const actual = await vi.importActual("@mui/material");
  return { ...(actual as object), useTheme: () => mockTheme };
});

describe("ErrorScreen", () => {
  describe("default rendering", () => {
    it('renders the "Something went wrong" heading', () => {
      render(<ErrorScreen />);
      expect(screen.getByText("Something went wrong")).toBeInTheDocument();
    });

    it("renders the default message", () => {
      render(<ErrorScreen />);
      expect(screen.getByText("Something went wrong.")).toBeInTheDocument();
    });

    it("does not render the retry button when onRetry is not provided", () => {
      render(<ErrorScreen />);
      expect(
        screen.queryByRole("button", { name: /try again/i }),
      ).not.toBeInTheDocument();
    });
  });

  describe("with a custom message", () => {
    it("renders the custom message", () => {
      render(<ErrorScreen message="Custom error message." />);
      expect(screen.getByText("Custom error message.")).toBeInTheDocument();
    });

    it("still renders the heading regardless of the message prop", () => {
      render(<ErrorScreen message="Custom error message." />);
      expect(screen.getByText("Something went wrong")).toBeInTheDocument();
    });
  });

  describe("with onRetry provided", () => {
    it('renders the "Try Again" button', () => {
      render(<ErrorScreen onRetry={vi.fn()} />);
      expect(
        screen.getByRole("button", { name: /try again/i }),
      ).toBeInTheDocument();
    });

    it("calls onRetry when the button is clicked", async () => {
      const onRetry = vi.fn();
      render(<ErrorScreen onRetry={onRetry} />);
      await userEvent.click(screen.getByRole("button", { name: /try again/i }));
      expect(onRetry).toHaveBeenCalledTimes(1);
    });
  });
});
