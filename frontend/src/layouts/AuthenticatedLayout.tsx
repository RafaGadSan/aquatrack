import { NavLink, Outlet } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'

const navLinkClassName = ({ isActive }: { isActive: boolean }) =>
  `rounded-md px-2 py-1 ${isActive ? 'bg-sky-100 text-sky-800' : 'text-slate-600 hover:bg-slate-100'}`

export function AuthenticatedLayout() {
  const { user, logout } = useAuth()

  return (
    <div className="min-h-screen bg-slate-50">
      <header className="flex flex-col gap-3 border-b border-slate-200 bg-white px-4 py-3 sm:flex-row sm:items-center sm:justify-between sm:px-6">
        <div className="flex flex-wrap items-center gap-x-6 gap-y-2">
          <span className="text-lg font-semibold text-slate-800">AquaTrack</span>
          <nav className="flex flex-wrap items-center gap-1 text-sm">
            <NavLink to="/" end className={navLinkClassName}>
              Dashboard
            </NavLink>
            <NavLink to="/facilities" className={navLinkClassName}>
              Facilities
            </NavLink>
            <NavLink to="/thresholds" className={navLinkClassName}>
              Thresholds
            </NavLink>
          </nav>
        </div>
        <div className="flex items-center justify-between gap-4 text-sm text-slate-600 sm:justify-end">
          <span className="truncate">
            {user?.fullName} <span className="text-slate-400">·</span> {user?.role}
          </span>
          <button
            onClick={logout}
            className="shrink-0 rounded-md border border-slate-300 px-3 py-1 hover:bg-slate-100"
          >
            Sign out
          </button>
        </div>
      </header>

      <main className="p-4 sm:p-6">
        <Outlet />
      </main>
    </div>
  )
}
