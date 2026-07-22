import { afterEach, describe, expect, it } from 'vitest'
import { AUTH_STORAGE_KEY, clearStoredAuth, readStoredAuth, writeStoredAuth } from './authStorage'
import type { AuthResponse } from '../types/auth'

const sampleAuth: AuthResponse = {
  token: 'abc123',
  expiresAt: '2026-07-23T00:00:00Z',
  userId: 'user-1',
  email: 'admin@aquatrack.dev',
  fullName: 'Admin Demo',
  role: 'Admin',
}

describe('authStorage', () => {
  afterEach(() => localStorage.clear())

  it('returns null when nothing is stored', () => {
    expect(readStoredAuth()).toBeNull()
  })

  it('round-trips a stored auth response', () => {
    writeStoredAuth(sampleAuth)
    expect(readStoredAuth()).toEqual(sampleAuth)
  })

  it('clears stored auth', () => {
    writeStoredAuth(sampleAuth)
    clearStoredAuth()
    expect(localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })

  it('returns null and clears storage when the stored value is invalid JSON', () => {
    localStorage.setItem(AUTH_STORAGE_KEY, 'not-json')

    expect(readStoredAuth()).toBeNull()
    expect(localStorage.getItem(AUTH_STORAGE_KEY)).toBeNull()
  })
})
