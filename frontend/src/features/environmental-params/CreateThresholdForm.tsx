import { useState, type FormEvent } from 'react'
import { useCreateThreshold } from './api'
import { useFacilities } from '../facilities/api'
import { getApiErrorMessage } from '../../lib/apiError'
import { ENVIRONMENTAL_PARAMETER_LABELS, type EnvironmentalParameter } from '../../types/environmentalParameter'

const PARAMETERS = Object.keys(ENVIRONMENTAL_PARAMETER_LABELS) as EnvironmentalParameter[]

export function CreateThresholdForm() {
  const createThreshold = useCreateThreshold()
  const { data: facilities } = useFacilities()
  const [parameter, setParameter] = useState<EnvironmentalParameter>('Temperature')
  const [minValue, setMinValue] = useState('')
  const [maxValue, setMaxValue] = useState('')
  const [facilityId, setFacilityId] = useState('')
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setError(null)
    try {
      await createThreshold.mutateAsync({
        parameter,
        minValue: Number(minValue),
        maxValue: Number(maxValue),
        facilityId: facilityId || null,
      })
      setMinValue('')
      setMaxValue('')
      setFacilityId('')
    } catch (err) {
      setError(getApiErrorMessage(err, 'Could not create the threshold.'))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="flex flex-wrap items-end gap-3 rounded-lg border border-slate-200 bg-white p-4">
      <div>
        <label htmlFor="threshold-parameter" className="block text-xs font-medium text-slate-600">
          Parameter
        </label>
        <select
          id="threshold-parameter"
          value={parameter}
          onChange={(e) => setParameter(e.target.value as EnvironmentalParameter)}
          className="mt-1 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        >
          {PARAMETERS.map((p) => (
            <option key={p} value={p}>
              {ENVIRONMENTAL_PARAMETER_LABELS[p]}
            </option>
          ))}
        </select>
      </div>

      <div>
        <label htmlFor="threshold-min" className="block text-xs font-medium text-slate-600">
          Min
        </label>
        <input
          id="threshold-min"
          type="number"
          step="0.01"
          required
          value={minValue}
          onChange={(e) => setMinValue(e.target.value)}
          className="mt-1 w-24 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        />
      </div>

      <div>
        <label htmlFor="threshold-max" className="block text-xs font-medium text-slate-600">
          Max
        </label>
        <input
          id="threshold-max"
          type="number"
          step="0.01"
          required
          value={maxValue}
          onChange={(e) => setMaxValue(e.target.value)}
          className="mt-1 w-24 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        />
      </div>

      <div>
        <label htmlFor="threshold-facility" className="block text-xs font-medium text-slate-600">
          Facility (optional)
        </label>
        <select
          id="threshold-facility"
          value={facilityId}
          onChange={(e) => setFacilityId(e.target.value)}
          className="mt-1 rounded-md border border-slate-300 px-2 py-1.5 text-sm focus:border-sky-500 focus:outline-none focus:ring-1 focus:ring-sky-500"
        >
          <option value="">Global (all facilities)</option>
          {facilities?.map((facility) => (
            <option key={facility.id} value={facility.id}>
              {facility.name}
            </option>
          ))}
        </select>
      </div>

      <button
        type="submit"
        disabled={createThreshold.isPending}
        className="rounded-md bg-sky-600 px-4 py-1.5 text-sm font-medium text-white hover:bg-sky-700 disabled:opacity-50"
      >
        {createThreshold.isPending ? 'Adding…' : 'Add threshold'}
      </button>

      {error && <p className="basis-full text-sm text-red-600">{error}</p>}
    </form>
  )
}
