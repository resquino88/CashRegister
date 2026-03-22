import { useDropzone } from 'react-dropzone'
import { Box, Paper, Stack, Typography, useTheme } from '@mui/material'
import {UploadFile as UploadFileIcon, CheckCircleOutline as CheckCircleOutlineIcon, Upload as UploadIcon} from '@mui/icons-material'
import type { Currency } from '@/endpoints'
import { useEffect } from 'react';

interface UploadAreaProps {
  file: File | undefined;
  setFile: React.Dispatch<React.SetStateAction<File | undefined>>;
  selectedCurrency: Currency | undefined;
  onFileUpload: (file: File) => Promise<void> | void;
}

export default function UploadArea({ file, setFile, selectedCurrency, onFileUpload }: UploadAreaProps) {
  const theme = useTheme()
  const disabled = !selectedCurrency

  const colors = {
    border:        theme.palette.divider,
    borderActive:  theme.palette.primary.main,
    borderSuccess: theme.palette.success.main,
    bgDefault:     theme.palette.background.default,
    bgHover:       theme.palette.action.hover,
    iconBg:        theme.palette.action.hover,
    iconColor:     theme.palette.text.secondary,
    iconDisabled:  theme.palette.text.disabled,
    iconSuccess:   theme.palette.success.main,
    subtitle:      theme.palette.text.secondary,
    filename:      theme.palette.text.primary,
  }

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: { 'text/plain': ['.txt', '.csv', '.tsv'] },
    maxFiles: 1,
    disabled,
    onDrop: (accepted) => {
      if (accepted[0]) {
        setFile(accepted[0])
      }
    },
  })

  useEffect(() => {
    if(file) {
      onFileUpload(file);
    }
  }, [file])

  return (
    <Paper variant="outlined" sx={{ p: 3 }}>
      <Stack direction="column" alignItems={"flex-start"}>
       <Stack direction="row" alignItems="center" spacing={1} mb={0.5}>
        <UploadIcon fontSize="small" />
        <Typography variant="subtitle1" fontWeight={600}>
          Process Transactions
        </Typography>
        </Stack>
        <Typography variant="body2" color={colors.subtitle} mb={2}>
          Upload a file with transaction data to calculate change
        </Typography>
      </Stack>

      <Box
        {...getRootProps()}
        sx={{
          border: '2px dashed',
          borderColor: isDragActive ? colors.borderActive : file ? colors.borderSuccess : colors.border,
          borderRadius: 2,
          p: { xs: 3, sm: 5 },
          textAlign: 'center',
          cursor: disabled ? 'not-allowed' : 'pointer',
          opacity: disabled ? 0.4 : 1,
          bgcolor: isDragActive ? colors.bgHover : colors.bgDefault,
          transition: 'border-color 0.2s, background-color 0.2s, opacity 0.2s',
          '&:hover': { borderColor: disabled ? colors.border : colors.borderActive },
        }}
      >
        <input {...getInputProps()} />
        {file
          ? <CheckCircleOutlineIcon sx={{ fontSize: 40, color: colors.iconSuccess, mb: 1 }} />
          : <UploadFileIcon sx={{ fontSize: 40, color: colors.iconDisabled, mb: 1 }} />
        }
        <Typography fontWeight={500} color={colors.filename}>
          {file ? file.name : disabled ? 'No currency has been selected' : 'Drag & drop a file here, or click to browse'}
        </Typography>
      </Box>
    </Paper>
  )
}