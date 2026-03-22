import type { Denomination } from '@/endpoints/denomination'
import apiClient from '@/endpoints/apiClient'

export async function getAllDenominations(): Promise<Denomination[]> {
  const res = await apiClient.get<Denomination[]>('/denomination')
  return res.data
}

export async function getDenominationById(id: number): Promise<Denomination> {
  const res = await apiClient.get<Denomination>(`/denomination/${id}`)
  return res.data
}

export async function getDenominationByName(name: string): Promise<Denomination> {
  const res = await apiClient.get<Denomination>(`/denomination/${encodeURIComponent(name)}`)
  return res.data
}
