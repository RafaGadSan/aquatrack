import type { EnvironmentalParameter } from './environmentalParameter'

export type AlertStatus = 'Active' | 'Resolved'

export const ALERT_STATUS_LABELS: Record<AlertStatus, string> = {
  Active: 'Activa',
  Resolved: 'Resuelta',
}

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
