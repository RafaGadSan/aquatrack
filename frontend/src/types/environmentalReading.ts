import type { AlertResponse } from './alert'

// Property name is lowercase "ph", not "pH": System.Text.Json's default camelCase policy
// collapses the two-letter acronym in the C# `PH` property down to a single lowercase letter.
// Verified against the real API response, not assumed.
export interface CreateEnvironmentalReadingRequest {
  temperature: number
  dissolvedOxygen: number
  salinity: number
  ph: number
}

export interface EnvironmentalReadingResponse {
  id: string
  facilityId: string
  recordedByUserId: string
  recordedAt: string
  temperature: number
  dissolvedOxygen: number
  salinity: number
  ph: number
}

export interface RecordReadingResult {
  reading: EnvironmentalReadingResponse
  triggeredAlerts: AlertResponse[]
}
