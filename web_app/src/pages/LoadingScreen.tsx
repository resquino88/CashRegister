import { Box, CircularProgress, Typography, useTheme } from "@mui/material";
import ReceiptLongIcon from "@mui/icons-material/ReceiptLong";

export default function LoadingScreen() {
  const theme = useTheme();

  const colors = {
    brand: theme.palette.primary.main,
    text: theme.palette.text.secondary,
    bg: theme.palette.background.default,
  };

  return (
    <Box
      sx={{
        height: "100vh",
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        justifyContent: "center",
        gap: 2,
        bgcolor: colors.bg,
      }}
    >
      <Box
        sx={{
          position: "relative",
          display: "flex",
          alignItems: "center",
          justifyContent: "center",
          mb: 1,
        }}
      >
        <CircularProgress
          size={64}
          thickness={2}
          sx={{ color: colors.brand }}
        />
        <ReceiptLongIcon
          sx={{ position: "absolute", fontSize: 26, color: colors.brand }}
        />
      </Box>
      <Typography variant="h6" fontWeight={600} color={colors.brand}>
        Cash Register
      </Typography>
      <Typography variant="body2" color={colors.text}>
        Loading, please wait...
      </Typography>
    </Box>
  );
}
