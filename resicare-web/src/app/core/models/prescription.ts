export type MedicationRoute = 'Oral' | 'Injectable' | 'Topical' | 'Other';

export const MEDICATION_ROUTE_LABELS: Record<MedicationRoute, string> = {
  Oral: 'Orale',
  Injectable: 'Injectable',
  Topical: 'Cutanée',
  Other: 'Autre',
};

export const MEDICATION_ROUTES: MedicationRoute[] = ['Oral', 'Injectable', 'Topical', 'Other'];

export type AdministrationStatus = 'Given' | 'Refused' | 'Omitted';

export const ADMINISTRATION_STATUS_LABELS: Record<AdministrationStatus, string> = {
  Given: 'Donné',
  Refused: 'Refusé',
  Omitted: 'Omis',
};

export interface PrescriptionDto {
  id: string;
  residentId: string;
  medicationName: string;
  dosage: string;
  posology: string;
  route: MedicationRoute;
  startDate: string;
  endDate: string | null;
  instructions: string | null;
  isDiscontinued: boolean;
}

export interface AdministrationDto {
  id: string;
  prescriptionId: string;
  medicationName: string;
  caregiverId: string;
  administeredAt: string;
  status: AdministrationStatus;
  notes: string | null;
}

export interface PrescribeMedicationRequest {
  medicationName: string;
  dosage: string;
  posology: string;
  route: MedicationRoute;
  startDate: string;
  endDate: string | null;
  instructions: string | null;
}

export interface RecordAdministrationRequest {
  status: AdministrationStatus;
  notes: string | null;
}
