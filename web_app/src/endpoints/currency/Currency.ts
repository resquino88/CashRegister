import type { Denomination } from "@/endpoints";

export interface Currency {
  id: number;
  name: string;
  currencySeparator: string;
  denominations: Denomination[];
}
