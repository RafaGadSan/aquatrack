export type EnvironmentalParameter = 'Temperature' | 'DissolvedOxygen' | 'Salinity' | 'PH'

export const ENVIRONMENTAL_PARAMETER_LABELS: Record<EnvironmentalParameter, string> = {
  Temperature: 'Temperatura',
  DissolvedOxygen: 'Oxígeno disuelto',
  Salinity: 'Salinidad',
  PH: 'pH',
}
