import type { Currency } from '@/endpoints/currency'

export interface Country {
  id: number
  name: string
  abbrevation: string
  currencyMultiplier: number
  currencies: Currency[]
}
