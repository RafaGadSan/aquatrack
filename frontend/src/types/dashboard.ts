import type { EnvironmentalParameter } from './environmentalParameter'
import type { FacilityStatus } from './facility'

export interface FacilityStatusCount {
  status: FacilityStatus
  count: number
}

export interface DashboardAlert {
  id: string
  facilityId: string
  facilityName: string
  parameter: EnvironmentalParameter
  value: number
  thresholdMin: number
  thresholdMax: number
  createdAt: string
}

export interface DashboardSummary {
  totalFacilities: number
  facilitiesByStatus: FacilityStatusCount[]
  activeAlertsCount: number
  activeAlerts: DashboardAlert[]
}
