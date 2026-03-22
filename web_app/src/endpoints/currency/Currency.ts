import type { Denomination } from "@/endpoints/denomination"

export interface Currency {
  id: number
  name: string
  currencySeparator: string
  denominations: Denomination[]
}
