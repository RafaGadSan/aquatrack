import { useState, type ChangeEvent, type FormEvent } from 'react'
import { useRecordReading } from './api'
import { getApiErrorMessage } from '../../lib/apiError'
import { ENVIRONMENTAL_PARAMETER_LABELS } from '../../types/environmentalParameter'
import type { AlertResponse } from '../../types/alert'

const initialForm = { temperature: '', dissolvedOxygen: '', salinity: '', ph: '' }

export function RecordReadingForm({ facilityId }: { facilityId: string }) {
  const recordReading = useRecordReading(facilityId)
  const [form, setForm] = useState(initialForm)
  const [error, setError] = useState<string | null>(null)
  const [triggeredAlerts, setTriggeredAlerts] = useState<AlertResponse[] | null>(null)

  function setField(field: keyof typeof initialForm) {
    return (event: ChangeEvent<HTMLInputElement>) => setForm((f) => ({ ...f, [field]: event.target.value }))
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setTriggeredAlerts(null)
    try {
      const result = await recordReading.mutateAsync({
        temperature: Number(form.temperature),
        dissolvedOxygen: Number(form.dissolvedOxygen),
        salinity: Number(form.salinity),
        ph: Number(form.ph),
      })
      setForm(initialForm)
      setTriggeredAlerts(result.triggeredAlerts)
    } catch (err) {
      setError(getApiErrorMessage(err, 'No se pudo registrar la lectura.'))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="rounded-lg border border-slate-200 bg-white p-4">
      <h2 className="mb-3 text-sm font-semibold text-slate-800">Registrar una lectura</h2>

      <div className="flex flex-wrap items-end gap-3">
        <div>
          <label htmlFor="temperature" className="block text-xs font-medium text-slate-600">
            Temperatura (°C)
          </label>
          <input
            id="temperature"
            type="number"
            step="0.01"
            required
            value={form.temperature}
            onChange={setField('temperature')}
            className="mt-1 w-28 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
          />
        </div>

        <div>
          <label htmlFor="dissolvedOxygen" className="block text-xs font-medium text-slate-600">
            Oxígeno disuelto (mg/L)
          </label>
          <input
            id="dissolvedOxygen"
            type="number"
            step="0.01"
            min="0"
            required
            value={form.dissolvedOxygen}
            onChange={setField('dissolvedOxygen')}
            className="mt-1 w-28 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
          />
        </div>

        <div>
          <label htmlFor="salinity" className="block text-xs font-medium text-slate-600">
            Salinidad (ppt)
          </label>
          <input
            id="salinity"
            type="number"
            step="0.01"
            min="0"
            required
            value={form.salinity}
            onChange={setField('salinity')}
            className="mt-1 w-28 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
          />
        </div>

        <div>
          <label htmlFor="ph" className="block text-xs font-medium text-slate-600">
            pH
          </label>
          <input
            id="ph"
            type="number"
            step="0.01"
            min="0"
            max="14"
            required
            value={form.ph}
            onChange={setField('ph')}
            className="mt-1 w-24 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
          />
        </div>

        <button
          type="submit"
          disabled={recordReading.isPending}
          className="rounded-md bg-sky-700 px-4 py-1.5 text-sm font-medium text-white hover:bg-sky-800 disabled:opacity-50"
        >
          {recordReading.isPending ? 'Registrando…' : 'Registrar'}
        </button>
      </div>

      {error && <p className="mt-3 text-sm text-red-600">{error}</p>}

      {triggeredAlerts && triggeredAlerts.length > 0 && (
        <div className="mt-3 rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-800">
          <p className="font-medium">Esta lectura disparó {triggeredAlerts.length} alerta(s):</p>
          <ul className="mt-1 list-inside list-disc">
            {triggeredAlerts.map((alert) => (
              <li key={alert.id}>
                {ENVIRONMENTAL_PARAMETER_LABELS[alert.parameter]}: {alert.value} (esperado {alert.thresholdMin}–{alert.thresholdMax})
              </li>
            ))}
          </ul>
        </div>
      )}

      {triggeredAlerts && triggeredAlerts.length === 0 && (
        <p className="mt-3 text-sm text-emerald-700">Lectura registrada, dentro de todos los umbrales aplicables.</p>
      )}
    </form>
  )
}
