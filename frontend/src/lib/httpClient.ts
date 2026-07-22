import axios from 'axios'
import { AUTH_STORAGE_KEY, readStoredAuth } from './authStorage'

export const httpClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
})

httpClient.interceptors.request.use((config) => {
  const auth = readStoredAuth()
  if (auth) {
    config.headers.Authorization = `Bearer ${auth.token}`
  }
  return config
})

// A 401 means the token is missing/expired/invalid — there's no refresh flow (see CLAUDE.md §7),
// so the only recovery is a fresh login. A hard redirect (rather than routing through React Router)
// keeps this interceptor decoupled from the router instance.
httpClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem(AUTH_STORAGE_KEY)
      if (window.location.pathname !== '/login') {
        window.location.href = '/login'
      }
    }
    return Promise.reject(error)
  },
)
