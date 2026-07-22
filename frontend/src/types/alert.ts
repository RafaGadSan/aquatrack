import type { EnvironmentalParameter } from './environmentalParameter'

export type AlertStatus = 'Active' | 'Resolved'

export interface AlertResponse {
  id: string
  facilityId: string
  environmentalReadingId: string
  parameter: EnvironmentalParameter
  value: number
  thresholdMin: number
  thresholdMax: number
  status: AlertStatus
  createdAt: string
  resolvedAt: string | null
}
