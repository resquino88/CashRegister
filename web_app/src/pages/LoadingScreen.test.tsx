import { render, screen } from "@testing-library/react";
import LoadingScreen from "./LoadingScreen";
import { mockTheme } from "@/test/fixtures";

vi.mock("@mui/material", async () => {
  const actual = await vi.importActual("@mui/material");
  return { ...(actual as object), useTheme: () => mockTheme };
});

describe("LoadingScreen", () => {
  it('renders the "Cash Register" brand text', () => {
    render(<LoadingScreen />);
    expect(screen.getByText("Cash Register")).toBeInTheDocument();
  });

  it("renders the loading message", () => {
    render(<LoadingScreen />);
    expect(screen.getByText("Loading, please wait...")).toBeInTheDocument();
  });

  it("renders a spinner", () => {
    render(<LoadingScreen />);
    expect(screen.getByRole("progressbar")).toBeInTheDocument();
  });
});
