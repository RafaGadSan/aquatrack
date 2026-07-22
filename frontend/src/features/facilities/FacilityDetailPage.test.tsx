import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { Link, MemoryRouter, Route, Routes } from 'react-router-dom'
import { FacilityDetailPage } from './FacilityDetailPage'
import { httpClient } from '../../lib/httpClient'
import type { Facility } from '../../types/facility'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn(), post: vi.fn(), put: vi.fn() },
}))

const facilityA: Facility = {
  id: 'facility-a',
  name: 'Cage A',
  type: 'Cage',
  status: 'Active',
  location: null,
  createdAt: '2026-07-22T00:00:00Z',
  updatedAt: '2026-07-22T00:00:00Z',
}

const facilityB: Facility = { ...facilityA, id: 'facility-b', name: 'Cage B' }

function renderDetailPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter initialEntries={['/facilities/facility-a']}>
        <nav>
          <Link to="/facilities/facility-a">Go to Cage A</Link>
          <Link to="/facilities/facility-b">Go to Cage B</Link>
        </nav>
        <Routes>
          <Route path="/facilities/:id" element={<FacilityDetailPage />} />
        </Routes>
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('FacilityDetailPage', () => {
  beforeEach(() => {
    vi.mocked(httpClient.get).mockReset().mockImplementation((url: string) => {
      if (url.endsWith('/readings') || url.endsWith('/alerts')) return Promise.resolve({ data: [] })
      if (url.endsWith('facility-b')) return Promise.resolve({ data: facilityB })
      return Promise.resolve({ data: facilityA })
    })
    vi.mocked(httpClient.post).mockReset().mockResolvedValue({
      data: {
        reading: { id: 'r1', facilityId: 'facility-a', recordedByUserId: 'u1', recordedAt: '2026-07-22T00:00:00Z', temperature: 28, dissolvedOxygen: 7, salinity: 30, ph: 7.5 },
        triggeredAlerts: [
          { id: 'a1', facilityId: 'facility-a', environmentalReadingId: 'r1', parameter: 'Temperature', value: 28, thresholdMin: 10, thresholdMax: 22, status: 'Active', createdAt: '2026-07-22T00:00:00Z', resolvedAt: null },
        ],
      },
    })
  })

  it('does not carry the previous facility\'s triggered-alerts banner over when returning to an already-cached facility', async () => {
    renderDetailPage()

    // Visit both facilities once first so both are cached by TanStack Query — otherwise the
    // isLoading branch's early return naturally unmounts the subtree on every navigation,
    // masking the bug this test exists to catch (it only shows up once there's cached data
    // for the destination, so the component doesn't pass through a loading state).
    expect(await screen.findByText('Cage A')).toBeInTheDocument()
    await userEvent.click(screen.getByRole('link', { name: /go to cage b/i }))
    expect(await screen.findByText('Cage B')).toBeInTheDocument()
    await userEvent.click(screen.getByRole('link', { name: /go to cage a/i }))
    expect(await screen.findByText('Cage A')).toBeInTheDocument()

    await userEvent.type(screen.getByLabelText(/temperature/i), '28')
    await userEvent.type(screen.getByLabelText(/dissolved oxygen/i), '7')
    await userEvent.type(screen.getByLabelText(/salinity/i), '30')
    await userEvent.type(screen.getByLabelText(/^ph$/i), '7.5')
    await userEvent.click(screen.getByRole('button', { name: /record/i }))
    expect(await screen.findByText(/triggered 1 alert/i)).toBeInTheDocument()

    await userEvent.click(screen.getByRole('link', { name: /go to cage b/i }))

    expect(await screen.findByText('Cage B')).toBeInTheDocument()
    expect(screen.queryByText(/triggered 1 alert/i)).not.toBeInTheDocument()
  })
})
