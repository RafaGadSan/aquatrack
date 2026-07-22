import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { RecordReadingForm } from './RecordReadingForm'
import { httpClient } from '../../lib/httpClient'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn(), post: vi.fn(), put: vi.fn() },
}))

function renderForm() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  render(
    <QueryClientProvider client={queryClient}>
      <RecordReadingForm facilityId="facility-1" />
    </QueryClientProvider>,
  )
}

async function fillAndSubmit() {
  await userEvent.type(screen.getByLabelText(/temperatura/i), '28')
  await userEvent.type(screen.getByLabelText(/oxígeno disuelto/i), '7')
  await userEvent.type(screen.getByLabelText(/salinidad/i), '30')
  await userEvent.type(screen.getByLabelText(/^ph$/i), '7.5')
  await userEvent.click(screen.getByRole('button', { name: /registrar/i }))
}

describe('RecordReadingForm', () => {
  beforeEach(() => {
    vi.mocked(httpClient.post).mockReset()
  })

  it('shows the triggered alerts returned by the API', async () => {
    vi.mocked(httpClient.post).mockResolvedValueOnce({
      data: {
        reading: { id: 'r1', facilityId: 'facility-1', recordedByUserId: 'u1', recordedAt: '2026-07-22T00:00:00Z', temperature: 28, dissolvedOxygen: 7, salinity: 30, ph: 7.5 },
        triggeredAlerts: [
          { id: 'a1', facilityId: 'facility-1', environmentalReadingId: 'r1', parameter: 'Temperature', value: 28, thresholdMin: 10, thresholdMax: 22, status: 'Active', createdAt: '2026-07-22T00:00:00Z', resolvedAt: null },
        ],
      },
    })

    renderForm()
    await fillAndSubmit()

    expect(await screen.findByText(/disparó 1 alerta/i)).toBeInTheDocument()
  })

  it('confirms the reading is within thresholds when no alerts trigger', async () => {
    vi.mocked(httpClient.post).mockResolvedValueOnce({
      data: {
        reading: { id: 'r1', facilityId: 'facility-1', recordedByUserId: 'u1', recordedAt: '2026-07-22T00:00:00Z', temperature: 18, dissolvedOxygen: 7, salinity: 30, ph: 7.5 },
        triggeredAlerts: [],
      },
    })

    renderForm()
    await fillAndSubmit()

    expect(await screen.findByText(/dentro de todos los umbrales aplicables/i)).toBeInTheDocument()
  })
})
