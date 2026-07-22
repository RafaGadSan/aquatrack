import { useAuth } from '../../context/AuthContext'
import { useFacilities } from '../facilities/api'
import { useThresholds } from './api'
import { CreateThresholdForm } from './CreateThresholdForm'
import { ENVIRONMENTAL_PARAMETER_LABELS } from '../../types/environmentalParameter'

export function ThresholdsPage() {
  const { user } = useAuth()
  const { data: thresholds, isLoading, isError } = useThresholds()
  const { data: facilities } = useFacilities()

  const facilityName = (facilityId: string | null) =>
    facilityId ? (facilities?.find((f) => f.id === facilityId)?.name ?? facilityId) : 'Global'

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-xl font-semibold text-slate-800">Umbrales de parámetros</h1>
        <p className="mt-1 text-sm text-slate-500">
          Rangos aceptables usados para evaluar las lecturas registradas. Un umbral específico de una instalación tiene
          prioridad sobre el global.
        </p>
      </div>

      {user?.role === 'Admin' && <CreateThresholdForm />}

      {isLoading && <p className="text-sm text-slate-500">Cargando umbrales…</p>}
      {isError && <p className="text-sm text-red-600">No se pudieron cargar los umbrales.</p>}
      {thresholds && thresholds.length === 0 && <p className="text-sm text-slate-500">Todavía no hay umbrales configurados.</p>}

      {thresholds && thresholds.length > 0 && (
        <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white">
          <table className="w-full text-sm">
            <thead className="bg-slate-50 text-left text-xs font-medium uppercase tracking-wide text-slate-500">
              <tr>
                <th className="px-4 py-2">Parámetro</th>
                <th className="px-4 py-2">Rango</th>
                <th className="px-4 py-2">Alcance</th>
              </tr>
            </thead>
            <tbody>
              {thresholds.map((threshold) => (
                <tr key={threshold.id} className="border-t border-slate-100">
                  <td className="px-4 py-2 font-medium text-slate-800">{ENVIRONMENTAL_PARAMETER_LABELS[threshold.parameter]}</td>
                  <td className="px-4 py-2 text-slate-600">
                    {threshold.minValue} – {threshold.maxValue}
                  </td>
                  <td className="px-4 py-2 text-slate-600">{facilityName(threshold.facilityId)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
