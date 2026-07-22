import { beforeEach, describe, expect, it, vi } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter } from 'react-router-dom'
import { AuthProvider } from '../../context/AuthContext'
import { LoginPage } from './LoginPage'
import { httpClient } from '../../lib/httpClient'

vi.mock('../../lib/httpClient', () => ({
  httpClient: { post: vi.fn() },
}))

function renderLoginPage() {
  render(
    <MemoryRouter>
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    </MemoryRouter>,
  )
}

describe('LoginPage', () => {
  beforeEach(() => {
    localStorage.clear()
    vi.mocked(httpClient.post).mockReset()
  })

  it('renders the login form', () => {
    renderLoginPage()

    expect(screen.getByLabelText(/email/i)).toBeInTheDocument()
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument()
  })

  it('shows an error message when login fails', async () => {
    vi.mocked(httpClient.post).mockRejectedValueOnce({
      isAxiosError: true,
      response: { data: { error: 'Invalid credentials.' } },
    })

    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/email/i), 'admin@aquatrack.dev')
    await userEvent.type(screen.getByLabelText(/password/i), 'wrong-password')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))

    expect(await screen.findByText('Invalid credentials.')).toBeInTheDocument()
  })

  it('persists the session when login succeeds', async () => {
    vi.mocked(httpClient.post).mockResolvedValueOnce({
      data: {
        token: 'abc123',
        expiresAt: new Date(Date.now() + 60_000).toISOString(),
        userId: 'user-1',
        email: 'admin@aquatrack.dev',
        fullName: 'Admin Demo',
        role: 'Admin',
      },
    })

    renderLoginPage()

    await userEvent.type(screen.getByLabelText(/email/i), 'admin@aquatrack.dev')
    await userEvent.type(screen.getByLabelText(/password/i), 'Admin123!')
    await userEvent.click(screen.getByRole('button', { name: /sign in/i }))

    await waitFor(() => {
      expect(localStorage.getItem('aquatrack.auth')).toContain('admin@aquatrack.dev')
    })
  })
})
