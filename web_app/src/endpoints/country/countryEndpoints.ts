import type { Country } from '@/endpoints/country'
import apiClient from '@/endpoints/apiClient'

export async function getAllCountries(): Promise<Country[]> {
  const res = await apiClient.get<Country[]>('/country')
  return res.data
}

export async function getCountryById(id: number): Promise<Country> {
  const res = await apiClient.get<Country>(`/country/${id}`)
  return res.data
}

export async function getCountryByName(name: string): Promise<Country> {
  const res = await apiClient.get<Country>(`/country/${encodeURIComponent(name)}`)
  return res.data
}
