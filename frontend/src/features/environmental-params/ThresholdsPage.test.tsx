import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { MemoryRouter } from 'react-router-dom'
import { AuthProvider } from '../../context/AuthContext'
import { ThresholdsPage } from './ThresholdsPage'
import { httpClient } from '../../lib/httpClient'
import { AUTH_STORAGE_KEY } from '../../lib/authStorage'
import type { AuthResponse } from '../../types/auth'
import type { ParameterThresholdResponse } from '../../types/parameterThreshold'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn(), post: vi.fn() },
}))

const thresholds: ParameterThresholdResponse[] = [
  { id: 't1', parameter: 'PH', minValue: 6.5, maxValue: 8.5, facilityId: null },
]

function seedAuth(role: AuthResponse['role']) {
  const auth: AuthResponse = {
    token: 'abc123',
    expiresAt: new Date(Date.now() + 60_000).toISOString(),
    userId: 'user-1',
    email: 'demo@aquatrack.dev',
    fullName: 'Demo User',
    role,
  }
  localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(auth))
}

function renderThresholdsPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <AuthProvider>
          <ThresholdsPage />
        </AuthProvider>
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('ThresholdsPage', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.mocked(httpClient.get).mockReset().mockImplementation((url: string) =>
      Promise.resolve({ data: url.includes('facilities') ? [] : thresholds }),
    )
  })

  it('shows the create form for an Admin', async () => {
    seedAuth('Admin')
    renderThresholdsPage()

    expect(await screen.findByText('pH')).toBeInTheDocument()
    expect(screen.getByLabelText(/^min$/i)).toBeInTheDocument()
  })

  it('hides the create form for an Operator', async () => {
    seedAuth('Operator')
    renderThresholdsPage()

    expect(await screen.findByText('pH')).toBeInTheDocument()
    expect(screen.queryByLabelText(/^min$/i)).not.toBeInTheDocument()
  })
})
