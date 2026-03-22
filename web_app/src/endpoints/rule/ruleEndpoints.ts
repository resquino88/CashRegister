import type { Rule } from '@/endpoints'
import apiClient from '@/endpoints/apiClient'

export async function getAllRules(): Promise<Rule[]> {
  const res = await apiClient.get<Rule[]>('/rule')
  return res.data
}

export async function getActiveRules(): Promise<Rule[]> {
  const res = await apiClient.get<Rule[]>('/rule/active')
  return res.data
}

export async function getRuleById(id: number): Promise<Rule> {
  const res = await apiClient.get<Rule>(`/rule/${id}`)
  return res.data
}

export async function getRuleByName(name: string): Promise<Rule> {
  const res = await apiClient.get<Rule>(`/rule/${encodeURIComponent(name)}`)
  return res.data
}
