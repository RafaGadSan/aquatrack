import { useQuery } from '@tanstack/react-query'
import { httpClient } from '../../lib/httpClient'
import type { DashboardSummary } from '../../types/dashboard'

async function fetchSummary(): Promise<DashboardSummary> {
  const { data } = await httpClient.get<DashboardSummary>('/api/dashboard/summary')
  return data
}

export function useDashboardSummary() {
  return useQuery({ queryKey: ['dashboard', 'summary'], queryFn: fetchSummary })
}
