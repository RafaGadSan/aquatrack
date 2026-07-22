import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { MemoryRouter } from 'react-router-dom'
import { AuthProvider } from '../../context/AuthContext'
import { FacilitiesPage } from './FacilitiesPage'
import { httpClient } from '../../lib/httpClient'
import { AUTH_STORAGE_KEY } from '../../lib/authStorage'
import type { AuthResponse } from '../../types/auth'
import type { Facility } from '../../types/facility'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { get: vi.fn(), post: vi.fn(), put: vi.fn() },
}))

const facilities: Facility[] = [
  {
    id: 'facility-1',
    name: 'Cage 1',
    type: 'Cage',
    status: 'Active',
    location: 'North bay',
    createdAt: '2026-07-22T00:00:00Z',
    updatedAt: '2026-07-22T00:00:00Z',
  },
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

function renderFacilitiesPage() {
  const queryClient = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  render(
    <QueryClientProvider client={queryClient}>
      <MemoryRouter>
        <AuthProvider>
          <FacilitiesPage />
        </AuthProvider>
      </MemoryRouter>
    </QueryClientProvider>,
  )
}

describe('FacilitiesPage', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.mocked(httpClient.get).mockReset().mockResolvedValue({ data: facilities })
  })

  it('shows the create form for an Admin', async () => {
    seedAuth('Admin')
    renderFacilitiesPage()

    expect(await screen.findByText('Cage 1')).toBeInTheDocument()
    expect(screen.getByLabelText(/^nombre$/i)).toBeInTheDocument()
    // One combobox for the create form's "Type" field, one for the row's status control.
    expect(screen.getAllByRole('combobox')).toHaveLength(2)
  })

  it('hides the create form and status control for an Operator', async () => {
    seedAuth('Operator')
    renderFacilitiesPage()

    expect(await screen.findByText('Cage 1')).toBeInTheDocument()
    expect(screen.queryByLabelText(/^nombre$/i)).not.toBeInTheDocument()
    expect(screen.queryByRole('combobox')).not.toBeInTheDocument()
    expect(screen.getByText('Activa')).toBeInTheDocument()
  })
})
