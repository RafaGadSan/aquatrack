import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { httpClient } from '../../lib/httpClient'
import type { CreateFacilityRequest, Facility, UpdateFacilityStatusRequest } from '../../types/facility'

const facilitiesKey = ['facilities'] as const

async function fetchFacilities(): Promise<Facility[]> {
  const { data } = await httpClient.get<Facility[]>('/api/facilities')
  return data
}

async function fetchFacility(id: string): Promise<Facility> {
  const { data } = await httpClient.get<Facility>(`/api/facilities/${id}`)
  return data
}

async function createFacility(request: CreateFacilityRequest): Promise<Facility> {
  const { data } = await httpClient.post<Facility>('/api/facilities', request)
  return data
}

async function updateFacilityStatus(id: string, request: UpdateFacilityStatusRequest): Promise<Facility> {
  const { data } = await httpClient.put<Facility>(`/api/facilities/${id}/status`, request)
  return data
}

export function useFacilities() {
  return useQuery({ queryKey: facilitiesKey, queryFn: fetchFacilities })
}

export function useFacility(id: string) {
  return useQuery({ queryKey: [...facilitiesKey, id], queryFn: () => fetchFacility(id) })
}

export function useCreateFacility() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: createFacility,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: facilitiesKey }),
  })
}

export function useUpdateFacilityStatus() {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: ({ id, request }: { id: string; request: UpdateFacilityStatusRequest }) => updateFacilityStatus(id, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: facilitiesKey }),
  })
}
