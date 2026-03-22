import { useMemo, useState } from "react";
import { getTheme } from "./theme";
import { ThemeProvider, CssBaseline } from "@mui/material";
import { LandingPage } from "@/pages";
import { Toaster } from "react-hot-toast";

export default function App() {
  const [mode, setMode] = useState<"light" | "dark">("light");
  const theme = useMemo(() => getTheme(mode), [mode]);

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Toaster
        position="bottom-right"
        toastOptions={{
          style: {
            fontFamily: "Inter, system-ui, sans-serif",
            borderRadius: "10px",
          },
        }}
      />
      <LandingPage
        onToggleMode={() => setMode((m) => (m === "light" ? "dark" : "light"))}
      />
    </ThemeProvider>
  );
}
