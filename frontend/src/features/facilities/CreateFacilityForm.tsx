import { useState, type FormEvent } from 'react'
import { useCreateFacility } from './api'
import { getApiErrorMessage } from '../../lib/apiError'
import { FACILITY_TYPE_LABELS, type FacilityType } from '../../types/facility'

const FACILITY_TYPES: FacilityType[] = ['Cage', 'Tank']

export function CreateFacilityForm() {
  const createFacility = useCreateFacility()
  const [name, setName] = useState('')
  const [type, setType] = useState<FacilityType>('Cage')
  const [location, setLocation] = useState('')
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    try {
      await createFacility.mutateAsync({ name, type, location: location.trim() || null })
      setName('')
      setLocation('')
    } catch (err) {
      setError(getApiErrorMessage(err, 'No se pudo crear la instalación.'))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-wrap items-end gap-3 rounded-lg border border-slate-200 bg-white p-4">
      <div>
        <label htmlFor="facility-name" className="block text-xs font-medium text-slate-600">
          Nombre
        </label>
        <input
          id="facility-name"
          required
          value={name}
          onChange={(e) => setName(e.target.value)}
          className="mt-1 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        />
      </div>

      <div>
        <label htmlFor="facility-type" className="block text-xs font-medium text-slate-600">
          Tipo
        </label>
        <select
          id="facility-type"
          value={type}
          onChange={(e) => setType(e.target.value as FacilityType)}
          className="mt-1 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        >
          {FACILITY_TYPES.map((t) => (
            <option key={t} value={t}>
              {FACILITY_TYPE_LABELS[t]}
            </option>
          ))}
        </select>
      </div>

      <div>
        <label htmlFor="facility-location" className="block text-xs font-medium text-slate-600">
          Ubicación (opcional)
        </label>
        <input
          id="facility-location"
          value={location}
          onChange={(e) => setLocation(e.target.value)}
          className="mt-1 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        />
      </div>

      <button
        type="submit"
        disabled={createFacility.isPending}
        className="rounded-md bg-sky-700 px-4 py-1.5 text-sm font-medium text-white hover:bg-sky-800 disabled:opacity-50"
      >
        {createFacility.isPending ? 'Agregando…' : 'Agregar instalación'}
      </button>

      {error && <p className="basis-full text-sm text-red-600">{error}</p>}
    </form>
  )
}
