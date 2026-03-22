import { useState, useEffect, useCallback } from 'react'
import { Container, Stack } from '@mui/material'
import { UploadInfo, UploadArea, Header, type HeaderState } from '@/components'
import { uploadFile, getAxiosError, getAllCountries, type Country } from '@/endpoints'
import { downloadBlob } from '@/utils'
import {LoadingScreen, ErrorScreen} from '@/pages'
import toast from 'react-hot-toast'

interface LandingPageProps {
  onToggleMode: () => void
}

export default function LandingPage({onToggleMode} : LandingPageProps) {
  const [countries, setCountries] = useState<Country[]>([]);
  const [file, setFile] = useState<File>();
  const [headerData, setHeaderData] = useState<HeaderState>();
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [errorMessage, setErrorMessage] = useState<string>();

  
  const setCountryData = useCallback(async () => {
    setIsLoading(true)
    let foundCountries: Country[] | undefined = undefined

    try {
      foundCountries = await getAllCountries()
      setCountries(foundCountries)
    } catch(ex) {
      const message = await getAxiosError(ex)
      toast.error(message)
      setErrorMessage(message)
    } finally {
      const defaultCountry = foundCountries?.at(0)
      const defaultCurrency = defaultCountry?.currencies.at(0)
      setHeaderData({ selectedCountry: defaultCountry, selectedCurrency: defaultCurrency })
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    setCountryData()
  }, [])

  const initiateFileUpload = async(file: File) => {
    if(!headerData || !headerData.selectedCountry || !headerData.selectedCurrency) return;
    const toastId = toast.loading('Processing file...');

    try {
      const result = await uploadFile(file, {countryId: headerData.selectedCountry.id, currencyId: headerData.selectedCurrency.id});
      downloadBlob(result, "results.txt");
      toast.success('Results downloaded successfully', {id: toastId})
    } catch(ex) {
      toast.error(await getAxiosError(ex), {id: toastId});
    } finally {
      setFile(undefined);
    }
  }

  const errorRetry = () => {
    setErrorMessage(undefined);
    setCountryData();
  }

  return (
    <>{isLoading ? <LoadingScreen /> : errorMessage !== undefined ? <ErrorScreen message={errorMessage} onRetry={errorRetry}/>: <>
      <Header
        countries={countries}
        headerData={headerData}
        setHeaderData={setHeaderData}
        onToggleMode={onToggleMode}
      />
      <Container maxWidth="lg" sx={{ py: 4 }}>
        <Stack spacing={3}>
          <UploadInfo decimalSeparator={headerData?.selectedCurrency?.currencySeparator ?? '.'}/>
          <UploadArea file={file} setFile={setFile} selectedCurrency={headerData?.selectedCurrency} onFileUpload={initiateFileUpload}/>
        </Stack>
      </Container>
    </>}</>);
}
