import { Link } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import { useFacilities, useUpdateFacilityStatus } from './api'
import { CreateFacilityForm } from './CreateFacilityForm'
import { FACILITY_STATUS_LABELS, FACILITY_TYPE_LABELS, type FacilityStatus } from '../../types/facility'

const STATUS_OPTIONS: FacilityStatus[] = ['Empty', 'Active', 'Harvesting']

const STATUS_STYLES: Record<FacilityStatus, string> = {
  Empty: 'bg-slate-100 text-slate-700',
  Active: 'bg-emerald-100 text-emerald-700',
  Harvesting: 'bg-amber-100 text-amber-700',
}

export function FacilitiesPage() {
  const { user } = useAuth()
  const { data: facilities, isLoading, isError } = useFacilities()
  const updateStatus = useUpdateFacilityStatus()

  const canCreate = user?.role === 'Admin'
  const canChangeStatus = user?.role === 'Admin' || user?.role === 'ShiftLead'

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-xl font-semibold text-slate-800">Instalaciones</h1>
        <p className="mt-1 text-sm text-slate-500">Jaulas y tanques registrados en AquaTrack.</p>
      </div>

      {canCreate && <CreateFacilityForm />}

      {isLoading && <p className="text-sm text-slate-500">Cargando instalaciones…</p>}
      {isError && <p className="text-sm text-red-600">No se pudieron cargar las instalaciones.</p>}

      {facilities && facilities.length === 0 && <p className="text-sm text-slate-500">Todavía no hay instalaciones.</p>}

      {facilities && facilities.length > 0 && (
        <div className="overflow-x-auto rounded-lg border border-slate-200 bg-white">
          <table className="w-full text-sm">
            <thead className="bg-slate-50 text-left text-xs font-medium uppercase tracking-wide text-slate-500">
              <tr>
                <th className="px-4 py-2">Nombre</th>
                <th className="px-4 py-2">Tipo</th>
                <th className="px-4 py-2">Ubicación</th>
                <th className="px-4 py-2">Estado</th>
              </tr>
            </thead>
            <tbody>
              {facilities.map((facility) => (
                <tr key={facility.id} className="border-t border-slate-100">
                  <td className="px-4 py-2 font-medium text-slate-800">
                    <Link to={`/facilities/${facility.id}`} className="text-sky-700 hover:underline">
                      {facility.name}
                    </Link>
                  </td>
                  <td className="px-4 py-2 text-slate-600">{FACILITY_TYPE_LABELS[facility.type]}</td>
                  <td className="px-4 py-2 text-slate-600">{facility.location ?? '—'}</td>
                  <td className="px-4 py-2">
                    {canChangeStatus ? (
                      <select
                        value={facility.status}
                        disabled={updateStatus.isPending}
                        onChange={(e) =>
                          updateStatus.mutate({ id: facility.id, request: { status: e.target.value as FacilityStatus } })
                        }
                        aria-label={`Estado de ${facility.name}`}
                        className={`rounded-full border-0 px-2 py-1 text-xs font-medium ${STATUS_STYLES[facility.status]}`}
                      >
                        {STATUS_OPTIONS.map((status) => (
                          <option key={status} value={status}>
                            {FACILITY_STATUS_LABELS[status]}
                          </option>
                        ))}
                      </select>
                    ) : (
                      <span className={`rounded-full px-2 py-1 text-xs font-medium ${STATUS_STYLES[facility.status]}`}>
                        {FACILITY_STATUS_LABELS[facility.status]}
                      </span>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
