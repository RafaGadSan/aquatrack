import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import { login as loginRequest } from '../features/auth/api'
import { clearStoredAuth, readStoredAuth, writeStoredAuth } from '../lib/authStorage'
import type { AuthResponse, LoginRequest, Role } from '../types/auth'

interface AuthUser {
  id: string
  email: string
  fullName: string
  role: Role
}

interface AuthContextValue {
  user: AuthUser | null
  isAuthenticated: boolean
  login: (request: LoginRequest) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined)

function toUser(auth: AuthResponse): AuthUser {
  return { id: auth.userId, email: auth.email, fullName: auth.fullName, role: auth.role }
}

function isExpired(auth: AuthResponse): boolean {
  return new Date(auth.expiresAt).getTime() <= Date.now()
}

function loadInitialUser(): AuthUser | null {
  const stored = readStoredAuth()
  if (!stored) return null
  if (isExpired(stored)) {
    clearStoredAuth()
    return null
  }
  return toUser(stored)
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(loadInitialUser)

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: user !== null,
      login: async (request) => {
        const data = await loginRequest(request)
        writeStoredAuth(data)
        setUser(toUser(data))
      },
      logout: () => {
        clearStoredAuth()
        setUser(null)
      },
    }),
    [user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider')
  }
  return context
}
