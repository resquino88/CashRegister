import type { Currency } from '@/endpoints'
import apiClient from '@/endpoints/apiClient'

export async function getAllCurrencies(): Promise<Currency[]> {
  const res = await apiClient.get<Currency[]>('/currency')
  return res.data
}

export async function getCurrencyById(id: number): Promise<Currency> {
  const res = await apiClient.get<Currency>(`/currency/${id}`)
  return res.data
}

export async function getCurrencyByName(name: string): Promise<Currency> {
  const res = await apiClient.get<Currency>(`/currency/${encodeURIComponent(name)}`)
  return res.data
}
