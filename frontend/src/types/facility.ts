export type FacilityType = 'Cage' | 'Tank'

export type FacilityStatus = 'Empty' | 'Active' | 'Harvesting'

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
