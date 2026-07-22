import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { MemoryRouter } from 'react-router-dom'
import { DashboardPage } from './DashboardPage'
import { httpClient } from '../../lib/httpClient'
import type { DashboardSummary } from '../../types/dashboard'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn() },
}))

function renderDashboard() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <DashboardPage />
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('DashboardPage', () => {
  beforeEach(() => {
    vi.mocked(httpClient.get).mockReset()
  })

  it('shows facility and alert counts, and links each active alert to its facility', async () => {
    const summary: DashboardSummary = {
      totalFacilities: 7,
      facilitiesByStatus: [
        { status: 'Empty', count: 0 },
        { status: 'Active', count: 3 },
        { status: 'Harvesting', count: 4 },
      ],
      activeAlertsCount: 1,
      activeAlerts: [
        {
          id: 'a1',
          facilityId: 'facility-1',
          facilityName: 'Cage 1',
          parameter: 'Temperature',
          value: 28,
          thresholdMin: 10,
          thresholdMax: 22,
          createdAt: '2026-07-22T00:00:00Z',
        },
      ],
    }
    vi.mocked(httpClient.get).mockResolvedValue({ data: summary })

    renderDashboard()

    expect(await screen.findByText('7')).toBeInTheDocument()
    expect(screen.getByText('3')).toBeInTheDocument()
    expect(screen.getByText('4')).toBeInTheDocument()
    expect(screen.getByText('1')).toBeInTheDocument()
    const alertLink = screen.getByRole('link', { name: 'Cage 1' })
    expect(alertLink).toHaveAttribute('href', '/facilities/facility-1')
  })

  it('shows an empty state when there are no active alerts', async () => {
    const summary: DashboardSummary = {
      totalFacilities: 0,
      facilitiesByStatus: [
        { status: 'Empty', count: 0 },
        { status: 'Active', count: 0 },
        { status: 'Harvesting', count: 0 },
      ],
      activeAlertsCount: 0,
      activeAlerts: [],
    }
    vi.mocked(httpClient.get).mockResolvedValue({ data: summary })

    renderDashboard()

    expect(await screen.findByText(/no active alerts/i)).toBeInTheDocument()
  })
})
