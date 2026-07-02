// Miroir des DTOs de pointage (les dates arrivent en chaînes ISO depuis l'API).
export interface TimeEntryDto {
  id: string;
  caregiverId: string;
  clockInAt: string;
  clockOutAt: string | null;
  durationMinutes: number | null;
}

// Vue « présences » du responsable : pointage + nom du soignant.
export interface CaregiverTimeEntryDto extends TimeEntryDto {
  caregiverFirstName: string;
  caregiverLastName: string;
}
