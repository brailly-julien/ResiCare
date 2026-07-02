export type CaregiverRole = 'Caregiver' | 'Manager';

export const CAREGIVER_ROLE_LABELS: Record<CaregiverRole, string> = {
  Caregiver: 'Soignant',
  Manager: 'Responsable',
};

export interface CaregiverDto {
  id: string;
  firstName: string;
  lastName: string;
  role: CaregiverRole;
}
