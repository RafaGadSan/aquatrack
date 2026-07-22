import { httpClient } from '../../lib/httpClient'
import type { AuthResponse, LoginRequest } from '../../types/auth'

export async function login(request: LoginRequest): Promise<AuthResponse> {
  const { data } = await httpClient.post<AuthResponse>('/api/auth/login', request)
  return data
}
