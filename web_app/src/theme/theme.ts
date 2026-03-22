import { createTheme, type Theme } from '@mui/material'
import { colors, shape, typography } from './tokens'

export const getTheme = (mode: 'light' | 'dark'): Theme =>
  createTheme({
    palette: {
      mode,
      primary: {
        light: colors.brand.light,
        main:  colors.brand.main,
        dark:  colors.brand.dark,
      },
      success: {
        main: colors.success.main,
      },
      error: {
        main: colors.error.main,
      },
      background: {
        default: mode === 'light' ? colors.grey[100]  : colors.grey[900],
        paper:   mode === 'light' ? colors.white        : colors.grey[800],
      },
    },
    shape,
    typography,
  })