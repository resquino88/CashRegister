import { Box, Button, Typography, useTheme } from "@mui/material";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutline";

interface ErrorScreenProps {
  message?: string;
  onRetry?: () => void;
}

export default function ErrorScreen({
  message = "Something went wrong.",
  onRetry,
}: ErrorScreenProps) {
  const theme = useTheme();

  const colors = {
    error: theme.palette.error.main,
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
      <ErrorOutlineIcon sx={{ fontSize: 56, color: colors.error }} />
      <Typography variant="h6" fontWeight={600} color={colors.error}>
        Something went wrong
      </Typography>
      <Typography variant="body2" color={colors.text}>
        {message}
      </Typography>
      {onRetry && (
        <Button
          variant="outlined"
          color="error"
          onClick={onRetry}
          sx={{ mt: 1 }}
        >
          Try Again
        </Button>
      )}
    </Box>
  );
}
