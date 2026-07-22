import { useAuth } from '../context/AuthContext'

// Placeholder landing page until the facilities/dashboard screens land in a later slice.
export function HomePage() {
  const { user } = useAuth()

  return (
    <div>
      <h1 className="text-xl font-semibold text-slate-800">Welcome, {user?.fullName}</h1>
      <p className="mt-2 text-sm text-slate-500">Facilities and environmental data screens are coming up next.</p>
    </div>
  )
}
