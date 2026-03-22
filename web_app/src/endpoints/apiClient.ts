import axios from 'axios'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL_HTTPS,
})

export async function getAxiosError(ex: unknown, fallback = 'Something went wrong.'): Promise<string> {
  if (axios.isAxiosError(ex) && ex.response?.data instanceof Blob) {
    return await ex.response.data.text()
  }
  return fallback
}

export default apiClient
