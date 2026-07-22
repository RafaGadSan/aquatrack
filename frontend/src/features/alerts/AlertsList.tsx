import { useAlerts } from './api'
import { ENVIRONMENTAL_PARAMETER_LABELS } from '../../types/environmentalParameter'
import { ALERT_STATUS_LABELS } from '../../types/alert'

export function AlertsList({ facilityId }: { facilityId: string }) {
  const { data: alerts, isLoading, isError } = useAlerts(facilityId)

  if (isLoading) return <p className="text-sm text-slate-500">Cargando alertas…</p>
  if (isError) return <p className="text-sm text-red-600">No se pudieron cargar las alertas.</p>
  if (!alerts || alerts.length === 0) return <p className="text-sm text-slate-500">No hay alertas para esta instalación.</p>

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
            <span className="text-xs uppercase tracking-wide">{ALERT_STATUS_LABELS[alert.status]}</span>
          </div>
          <p className="mt-0.5 text-xs">
            Lectura: {alert.value} (esperado {alert.thresholdMin}–{alert.thresholdMax})
          </p>
        </li>
      ))}
    </ul>
  )
}
