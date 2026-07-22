import { useQuery } from '@tanstack/react-query'
import { httpClient } from '../../lib/httpClient'
import type { AlertResponse } from '../../types/alert'

// Deliberately not nested under ['facilities', ...]: alerts are their own resource, just filtered
// by facility. Sharing that prefix would mean invalidating the facilities list (on create/status
// change) also invalidates every facility's alerts, since TanStack Query matches by prefix.
export const alertsKey = (facilityId: string) => ['alerts', facilityId] as const

async function fetchAlerts(facilityId: string): Promise<AlertResponse[]> {
  const { data } = await httpClient.get<AlertResponse[]>(`/api/facilities/${facilityId}/alerts`)
  return data
}

export function useAlerts(facilityId: string) {
  return useQuery({ queryKey: alertsKey(facilityId), queryFn: () => fetchAlerts(facilityId) })
}
