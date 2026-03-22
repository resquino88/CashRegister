import {
  AppBar, Toolbar, Stack, Typography,
  FormControl, InputLabel, Select, MenuItem, Switch,
  useTheme,
} from '@mui/material'
import {ReceiptLong as ReceiptLongIcon, LightMode as LightModeIcon, DarkMode as DarkModeIcon} from '@mui/icons-material'
import type { Country, Currency } from '@/endpoints'

export interface HeaderState {
  selectedCountry?: Country;
  selectedCurrency?: Currency;
}

interface HeaderProps {
  countries: Country[]
  headerData: HeaderState | undefined;
  setHeaderData: React.Dispatch<React.SetStateAction<HeaderState | undefined>>;
  onToggleMode: () => void;
}


export default function Header({
  countries, headerData, setHeaderData, onToggleMode
}: HeaderProps) {
  const theme = useTheme()
  const mode = theme.palette.mode

  const colors = {
    appBarBg:   theme.palette.background.paper,
    border:     theme.palette.divider,
    brand:      theme.palette.primary.main,
    textPrimary: theme.palette.text.primary,
    iconMuted:  theme.palette.text.secondary,
  }

  const setSelectedCountry = (selectedCountryId: number) => {
    const foundCountry = countries.find(c => c.id === selectedCountryId);
    const defaultCurrency = foundCountry?.currencies.at(0) ?? undefined;

    setHeaderData({ selectedCurrency: defaultCurrency, selectedCountry: foundCountry });
  }

  const setSelectedCurrency = (selectedCurrencyId: number) => {
    if(headerData === undefined || headerData.selectedCountry === undefined) return;

    const selectedCountry = headerData.selectedCountry;
    const foundCurrency = selectedCountry.currencies.find(c => c.id === selectedCurrencyId);
    setHeaderData(prev => ({ ...prev, selectedCurrency: foundCurrency }));
  }

  return (
    <AppBar position="sticky" elevation={0} sx={{ borderBottom: 1, borderColor: colors.border, bgcolor: colors.appBarBg }}>
      <Toolbar sx={{ gap: 2, flexWrap: 'wrap', py: { xs: 1, sm: 0 } }}>

        <Stack direction="row" alignItems="center" spacing={1} sx={{ mr: 'auto' }}>
          <ReceiptLongIcon sx={{ color: colors.brand }} />
          <Typography variant="h6" fontWeight={700} color={colors.textPrimary} noWrap>
            Cash Register
          </Typography>
        </Stack>

        <FormControl size="small" sx={{ minWidth: 200 }}>
          <InputLabel>Country</InputLabel>
          <Select value={headerData?.selectedCountry?.id ?? ''} label="Country" onChange={(e) => setSelectedCountry(Number(e.target.value))}>
            {countries.map(c => (
              <MenuItem key={c.id} value={c.id}>{c.name} ({c.abbrevation})</MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl size="small" sx={{ minWidth: 120 }} disabled={!headerData}>
          <InputLabel>Currency</InputLabel>
          <Select value={headerData?.selectedCurrency?.id ?? ''} label="Currency" onChange={(e) => setSelectedCurrency(Number(e.target.value))}>
            {(headerData?.selectedCountry?.currencies ?? []).map(c => (
              <MenuItem key={c.id} value={c.id}>{c.name}</MenuItem>
            ))}
          </Select>
        </FormControl>
        <Stack direction="row" alignItems="center" spacing={0.5}>
          <LightModeIcon fontSize="small" sx={{ color: colors.iconMuted }} />
          <Switch checked={mode === 'dark'} onChange={onToggleMode} size="small" />
          <DarkModeIcon fontSize="small" sx={{ color: colors.iconMuted }} />
        </Stack>
      </Toolbar>
    </AppBar>
  )
}
