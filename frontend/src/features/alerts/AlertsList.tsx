import { useAlerts } from './api'
import { ENVIRONMENTAL_PARAMETER_LABELS } from '../../types/environmentalParameter'

export function AlertsList({ facilityId }: { facilityId: string }) {
  const { data: alerts, isLoading, isError } = useAlerts(facilityId)

  if (isLoading) return <p className="text-sm text-slate-500">Loading alerts…</p>
  if (isError) return <p className="text-sm text-red-600">Could not load alerts.</p>
  if (!alerts || alerts.length === 0) return <p className="text-sm text-slate-500">No alerts for this facility.</p>

  return (
    <ul className="space-y-2">
      {alerts.map((alert) => (
        <li
          key={alert.id}
          className={`rounded-md border px-3 py-2 text-sm ${
            alert.status === 'Active' ? 'border-red-200 bg-red-50 text-red-800' : 'border-slate-200 bg-slate-50 text-slate-500'
          }`}
        >
          <div className="flex items-center justify-between">
            <span className="font-medium">{ENVIRONMENTAL_PARAMETER_LABELS[alert.parameter]}</span>
            <span className="text-xs uppercase tracking-wide">{alert.status}</span>
          </div>
          <p className="mt-0.5 text-xs">
            Reading: {alert.value} (expected {alert.thresholdMin}–{alert.thresholdMax})
          </p>
        </li>
      ))}
    </ul>
  )
}
