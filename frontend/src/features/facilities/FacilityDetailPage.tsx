import { Link, useParams } from 'react-router-dom'
import { useFacility } from './api'
import { RecordReadingForm } from '../environmental-params/RecordReadingForm'
import { ReadingsList } from '../environmental-params/ReadingsList'
import { AlertsList } from '../alerts/AlertsList'

export function FacilityDetailPage() {
  const { id } = useParams<{ id: string }>()
  const { data: facility, isLoading, isError } = useFacility(id!)

  if (isLoading) return <p className="text-sm text-slate-500">Loading facility…</p>
  if (isError || !facility) return <p className="text-sm text-red-600">Could not load this facility.</p>

  return (
    <div className="space-y-6">
      <div>
        <Link to="/" className="text-sm text-sky-600 hover:underline">
          ← Back to facilities
        </Link>
        <h1 className="mt-1 text-xl font-semibold text-slate-800">{facility.name}</h1>
        <p className="mt-1 text-sm text-slate-500">
          {facility.type} · {facility.status} {facility.location ? `· ${facility.location}` : ''}
        </p>
      </div>

      <RecordReadingForm facilityId={facility.id} />

      <div className="grid gap-6 md:grid-cols-2">
        <div>
          <h2 className="mb-2 text-sm font-semibold text-slate-800">Recent readings</h2>
          <ReadingsList facilityId={facility.id} />
        </div>
        <div>
          <h2 className="mb-2 text-sm font-semibold text-slate-800">Alerts</h2>
          <AlertsList facilityId={facility.id} />
        </div>
      </div>
    </div>
  )
}
