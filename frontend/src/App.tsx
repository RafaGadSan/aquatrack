import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider } from './context/AuthContext'
import { AuthenticatedLayout } from './layouts/AuthenticatedLayout'
import { LoginPage } from './features/auth/LoginPage'
import { ProtectedRoute } from './routes/ProtectedRoute'
import { FacilitiesPage } from './features/facilities/FacilitiesPage'
import { FacilityDetailPage } from './features/facilities/FacilityDetailPage'
import { ThresholdsPage } from './features/environmental-params/ThresholdsPage'

const queryClient = new QueryClient()

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <AuthProvider>
          <Routes>
            <Route path="/login" element={<LoginPage />} />
            <Route element={<ProtectedRoute />}>
              <Route element={<AuthenticatedLayout />}>
                <Route path="/" element={<FacilitiesPage />} />
                <Route path="/facilities/:id" element={<FacilityDetailPage />} />
                <Route path="/thresholds" element={<ThresholdsPage />} />
              </Route>
            </Route>
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </AuthProvider>
      </BrowserRouter>
    </QueryClientProvider>
  )
}

export default App
