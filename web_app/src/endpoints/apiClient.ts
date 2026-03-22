import axios from 'axios'

const useHttps = import.meta.env.VITE_USE_HTTPS === 'true'
const url = useHttps ? import.meta.env.VITE_API_URL_HTTPS : import.meta.env.VITE_API_URL;

const apiClient = axios.create({
  baseURL: url,
})

export async function getAxiosError(ex: unknown, fallback = 'Something went wrong.'): Promise<string> {
  if (axios.isAxiosError(ex) && ex.response?.data instanceof Blob) {
    return await ex.response.data.text()
  }
  return fallback
}

export default apiClient
