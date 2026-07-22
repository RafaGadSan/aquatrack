import { Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

export function AuthenticatedLayout() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="flex items-center justify-between border-b border-slate-200 bg-white px-6 py-3">
        <span className="text-lg font-semibold text-slate-800">AquaTrack</span>
        <div className="flex items-center gap-4 text-sm text-slate-600">
          <span>
            {user?.fullName} <span className="text-slate-400">·</span> {user?.role}
          </span>
          <button onClick={logout} className="rounded-md border border-slate-300 px-3 py-1 hover:bg-slate-100">
            Sign out
          </button>
        </div>
      </header>

      <main className="p-6">
        <Outlet />
      </main>
    </div>
  )
}
