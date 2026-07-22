export type FacilityType = 'Cage' | 'Tank'

export type FacilityStatus = 'Empty' | 'Active' | 'Harvesting'

export const FACILITY_TYPE_LABELS: Record<FacilityType, string> = {
  Cage: 'Jaula',
  Tank: 'Tanque',
}

export const FACILITY_STATUS_LABELS: Record<FacilityStatus, string> = {
  Empty: 'Vacía',
  Active: 'Activa',
  Harvesting: 'En cosecha',
}

export interface Facility {
  id: string
  name: string
  type: FacilityType
  status: FacilityStatus
  location: string | null
  createdAt: string
  updatedAt: string
}

export interface CreateFacilityRequest {
  name: string
  type: FacilityType
  location: string | null
}

export interface UpdateFacilityStatusRequest {
  status: FacilityStatus
}
