import { useReadings } from './api'

export function ReadingsList({ facilityId }: { facilityId: string }) {
  const { data: readings, isLoading, isError } = useReadings(facilityId)

  if (isLoading) return <p className="text-sm text-slate-500">Loading readings…</p>
  if (isError) return <p className="text-sm text-red-600">Could not load readings.</p>
  if (!readings || readings.length === 0) return <p className="text-sm text-slate-500">No readings recorded yet.</p>

  return (
    <table className="w-full overflow-hidden rounded-lg border border-slate-200 bg-white text-sm">
      <thead className="bg-slate-50 text-left text-xs font-medium uppercase tracking-wide text-slate-500">
        <tr>
          <th className="px-4 py-2">Recorded at</th>
          <th className="px-4 py-2">Temp (°C)</th>
          <th className="px-4 py-2">O₂ (mg/L)</th>
          <th className="px-4 py-2">Salinity (ppt)</th>
          <th className="px-4 py-2">pH</th>
        </tr>
      </thead>
      <tbody>
        {readings.map((reading) => (
          <tr key={reading.id} className="border-t border-slate-100">
            <td className="px-4 py-2 text-slate-600">{new Date(reading.recordedAt).toLocaleString()}</td>
            <td className="px-4 py-2 text-slate-800">{reading.temperature}</td>
            <td className="px-4 py-2 text-slate-800">{reading.dissolvedOxygen}</td>
            <td className="px-4 py-2 text-slate-800">{reading.salinity}</td>
            <td className="px-4 py-2 text-slate-800">{reading.ph}</td>
          </tr>
        ))}
      </tbody>
    </table>
  )
}
