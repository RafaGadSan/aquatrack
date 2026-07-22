import { Link } from 'react-router-dom'
import { useDashboardSummary } from './api'
import { ENVIRONMENTAL_PARAMETER_LABELS } from '../../types/environmentalParameter'

function StatTile({ label, value, critical }: { label: string; value: number; critical?: boolean }) {
  return (
    <div className="rounded-lg border border-slate-200 bg-white p-4">
      <p className="text-xs font-medium uppercase tracking-wide text-slate-500">{label}</p>
      <p className={`mt-1 text-3xl font-semibold ${critical && value > 0 ? 'text-red-600' : 'text-slate-800'}`}>{value}</p>
    </div>
  )
}

export function DashboardPage() {
  const { data: summary, isLoading, isError } = useDashboardSummary()

  if (isLoading) return <p className="text-sm text-slate-500">Loading dashboard…</p>
  if (isError || !summary) return <p className="text-sm text-red-600">Could not load the dashboard.</p>

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-xl font-semibold text-slate-800">Dashboard</h1>
        <p className="mt-1 text-sm text-slate-500">Operational snapshot across all facilities.</p>
      </div>

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-5">
        <StatTile label="Facilities" value={summary.totalFacilities} />
        {summary.facilitiesByStatus.map((s) => (
          <StatTile key={s.status} label={s.status} value={s.count} />
        ))}
        <StatTile label="Active alerts" value={summary.activeAlertsCount} critical />
      </div>

      <div>
        <h2 className="mb-2 text-sm font-semibold text-slate-800">Active alerts</h2>
        {summary.activeAlerts.length === 0 ? (
          <p className="text-sm text-slate-500">No active alerts across any facility.</p>
        ) : (
          <ul className="space-y-2">
            {summary.activeAlerts.map((alert) => (
              <li key={alert.id} className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800">
                <div className="flex items-center justify-between">
                  <Link to={`/facilities/${alert.facilityId}`} className="font-medium hover:underline">
                    {alert.facilityName}
                  </Link>
                  <span className="text-xs uppercase tracking-wide">{ENVIRONMENTAL_PARAMETER_LABELS[alert.parameter]}</span>
                </div>
                <p className="mt-0.5 text-xs">
                  Reading: {alert.value} (expected {alert.thresholdMin}–{alert.thresholdMax})
                </p>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}
