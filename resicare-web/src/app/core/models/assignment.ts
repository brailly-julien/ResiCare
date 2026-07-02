export type Shift = 'Morning' | 'Afternoon' | 'Night';

export const SHIFT_LABELS: Record<Shift, string> = {
  Morning: 'Matin',
  Afternoon: 'Après-midi',
  Night: 'Nuit',
};

export const SHIFTS: Shift[] = ['Morning', 'Afternoon', 'Night'];

// Vue planning du responsable (affectation + noms).
export interface AssignmentDetailDto {
  id: string;
  caregiverId: string;
  caregiverFirstName: string;
  caregiverLastName: string;
  residentId: string;
  residentFirstName: string;
  residentLastName: string;
  roomNumber: string;
  date: string;
  shift: Shift;
}

// Vue soignant : « mes résidents du jour ».
export interface AssignmentResidentDto {
  id: string;
  residentId: string;
  residentFirstName: string;
  residentLastName: string;
  roomNumber: string;
  date: string;
  shift: Shift;
}

export interface CreateAssignmentRequest {
  caregiverId: string;
  residentId: string;
  date: string;
  shift: Shift;
}
