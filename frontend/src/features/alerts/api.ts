import { useQuery } from '@tanstack/react-query'
import { httpClient } from '../../lib/httpClient'
import type { AlertResponse } from '../../types/alert'

export const alertsKey = (facilityId: string) => ['facilities', facilityId, 'alerts'] as const

async function fetchAlerts(facilityId: string): Promise<AlertResponse[]> {
  const { data } = await httpClient.get<AlertResponse[]>(`/api/facilities/${facilityId}/alerts`)
  return data
}

export function useAlerts(facilityId: string) {
  return useQuery({ queryKey: alertsKey(facilityId), queryFn: () => fetchAlerts(facilityId) })
}
