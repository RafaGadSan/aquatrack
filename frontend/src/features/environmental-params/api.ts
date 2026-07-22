import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { httpClient } from '../../lib/httpClient'
import type { CreateEnvironmentalReadingRequest, EnvironmentalReadingResponse, RecordReadingResult } from '../../types/environmentalReading'
import type { CreateParameterThresholdRequest, ParameterThresholdResponse } from '../../types/parameterThreshold'
import { alertsKey } from '../alerts/api'

// Same reasoning as alertsKey in features/alerts/api.ts: kept out of the ['facilities', ...]
// prefix so invalidating the facilities list doesn't also invalidate every facility's readings.
const readingsKey = (facilityId: string) => ['readings', facilityId] as const
const thresholdsKey = ['parameter-thresholds'] as const

async function fetchReadings(facilityId: string): Promise<EnvironmentalReadingResponse[]> {
  const { data } = await httpClient.get<EnvironmentalReadingResponse[]>(`/api/facilities/${facilityId}/readings`)
  return data
}

async function recordReading(facilityId: string, request: CreateEnvironmentalReadingRequest): Promise<RecordReadingResult> {
  const { data } = await httpClient.post<RecordReadingResult>(`/api/facilities/${facilityId}/readings`, request)
  return data
}

async function fetchThresholds(): Promise<ParameterThresholdResponse[]> {
  const { data } = await httpClient.get<ParameterThresholdResponse[]>('/api/parameter-thresholds')
  return data
}

async function createThreshold(request: CreateParameterThresholdRequest): Promise<ParameterThresholdResponse> {
  const { data } = await httpClient.post<ParameterThresholdResponse>('/api/parameter-thresholds', request)
  return data
}

export function useReadings(facilityId: string) {
  return useQuery({ queryKey: readingsKey(facilityId), queryFn: () => fetchReadings(facilityId) })
}

export function useRecordReading(facilityId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (request: CreateEnvironmentalReadingRequest) => recordReading(facilityId, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: readingsKey(facilityId) })
      queryClient.invalidateQueries({ queryKey: alertsKey(facilityId) })
    },
  })
}

export function useThresholds() {
  return useQuery({ queryKey: thresholdsKey, queryFn: fetchThresholds })
}

export function useCreateThreshold() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createThreshold,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: thresholdsKey }),
  })
}
