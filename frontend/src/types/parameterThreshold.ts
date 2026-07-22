import type { EnvironmentalParameter } from './environmentalParameter'

export interface CreateParameterThresholdRequest {
  parameter: EnvironmentalParameter
  minValue: number
  maxValue: number
  facilityId: string | null
}

export interface ParameterThresholdResponse {
  id: string
  parameter: EnvironmentalParameter
  minValue: number
  maxValue: number
  facilityId: string | null
}
