export type EnvironmentalParameter = 'Temperature' | 'DissolvedOxygen' | 'Salinity' | 'PH'

export const ENVIRONMENTAL_PARAMETER_LABELS: Record<EnvironmentalParameter, string> = {
  Temperature: 'Temperature',
  DissolvedOxygen: 'Dissolved oxygen',
  Salinity: 'Salinity',
  PH: 'pH',
}
