export type Role = 'Admin' | 'ShiftLead' | 'Operator'

export const ROLE_LABELS: Record<Role, string> = {
  Admin: 'Administrador',
  ShiftLead: 'Jefe de turno',
  Operator: 'Operario',
}

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
