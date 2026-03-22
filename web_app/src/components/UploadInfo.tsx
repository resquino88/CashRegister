import { Box, Paper, Stack, Typography, List, ListItem, ListItemIcon, useTheme } from '@mui/material'
import {InfoOutlined as InfoOutlinedIcon, FiberManualRecord as FiberManualRecordIcon} from '@mui/icons-material'

interface UploadInfoProps {
  decimalSeparator: string;
}

export default function UploadInfo({decimalSeparator} : UploadInfoProps) {
  const theme = useTheme()

  const colors = {
    brand:       theme.palette.primary.main,
    subtitle:    theme.palette.text.secondary,
    text:        theme.palette.text.primary,
    codeBg:      theme.palette.action.hover,
    codeText:    theme.palette.text.primary,
    bullet:      theme.palette.primary.main,
    noteText:    theme.palette.text.secondary,
  }

  const lines = [`2${decimalSeparator}13,3${decimalSeparator}00`, `1${decimalSeparator}97,2${decimalSeparator}00`, `3${decimalSeparator}33,5${decimalSeparator}00`]

  const notes = [
    'Supports .txt, .csv, .tsv file formats',
    'Each line produces one line of output',
    'Select the appropriate country and currency before uploading',
    `If a currency supports decimals, both amounts must always include decimals, even if zero. ${decimalSeparator ? `(e.g. 3${decimalSeparator}00` : ''})`,
  ]

  return (
    <Paper variant="outlined" sx={{ p: 3 }}>

      {/* Header */}
      <Stack direction="row" alignItems="center" spacing={1} mb={0.5}>
        <InfoOutlinedIcon fontSize="small" sx={{ color: colors.brand }} />
        <Typography variant="subtitle1" fontWeight={600} color={colors.text}>
          Guidelines
        </Typography>
      </Stack>
      <Typography variant="body2" color={colors.subtitle} mb={3}>
        Instructions for formatting your transaction file
      </Typography>

      {/* File Format */}
      <Typography variant="subtitle2" fontWeight={700} color={colors.text} mb={1}>
        File Format
      </Typography>
      <Typography variant="body2" color={colors.subtitle} mb={1.5}>
        Each line should contain the amount owed and amount paid.
      </Typography>
      <Box sx={{ bgcolor: colors.codeBg, borderRadius: 1, px: 2, py: 1.5, mb: 3, fontFamily: 'monospace', fontSize: '0.85rem', color: colors.codeText }}>
        {lines.map((line, i) => (
          <Box key={i}>{line}</Box>
        ))}
      </Box>

      {/* Notes */}
      <Typography variant="subtitle2" fontWeight={700} color={colors.text} mb={0.5}>
        Notes
      </Typography>
      <List dense disablePadding>
        {notes.map((note, i) => (
          <ListItem key={i} disableGutters sx={{ py: 0.3, alignItems: 'center' }}>
            <ListItemIcon sx={{ minWidth: 24 }}>
              <FiberManualRecordIcon sx={{ fontSize: 7, color: colors.bullet }} />
            </ListItemIcon>
            <Typography variant="body2" color={colors.noteText}>{note}</Typography>
          </ListItem>
        ))}
      </List>
    </Paper>
  )
}