import { describe, expect, it, vi } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import type { ReactNode } from 'react'
import { useReadings } from './api'
import { useCreateFacility } from '../facilities/api'
import { httpClient } from '../../lib/httpClient'

// Regression test for a query-key design issue found during PR review: readings/alerts keys used
// to be nested under ['facilities', facilityId, ...], sharing a prefix with the facilities list key
// (['facilities']). TanStack Query's invalidateQueries matches by prefix by default, so creating or
// updating a facility was also invalidating (and refetching, if mounted) every facility's readings
// and alerts. Fixed by giving readings/alerts their own top-level key namespace.
vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn(), post: vi.fn() },
}))

describe('facilities/readings query key isolation', () => {
  it('does not refetch a mounted facility\'s readings when a facility is created', async () => {
    const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
    vi.mocked(httpClient.get).mockResolvedValue({ data: [] })
    vi.mocked(httpClient.post).mockResolvedValue({
      data: { id: 'new-facility', name: 'New', type: 'Cage', status: 'Empty', location: null, createdAt: '', updatedAt: '' },
    })

    const wrapper = ({ children }: { children: ReactNode }) => (
      <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
    )

    const { result: readings } = renderHook(() => useReadings('facility-1'), { wrapper })
    await waitFor(() => expect(readings.current.isSuccess).toBe(true))
    expect(httpClient.get).toHaveBeenCalledTimes(1)

    const { result: createFacility } = renderHook(() => useCreateFacility(), { wrapper })
    await createFacility.current.mutateAsync({ name: 'New', type: 'Cage', location: null })

    // The mutation's onSuccess invalidates the facilities list; if readings shared that key
    // prefix, this would trigger a second GET for facility-1's readings.
    expect(httpClient.get).toHaveBeenCalledTimes(1)
  })
})
