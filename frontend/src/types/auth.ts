export type Role = 'Admin' | 'ShiftLead' | 'Operator'

export interface LoginRequest {
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  expiresAt: string
  userId: string
  email: string
  fullName: string
  role: Role
}
