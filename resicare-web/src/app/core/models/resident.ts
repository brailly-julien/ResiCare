// ============================================================
//  Modèles TypeScript — miroir des DTOs de l'API .NET.
//  Les enums sont sérialisés en TEXTE par l'API -> types union de chaînes.
// ============================================================

export type AutonomyLevel = 'Independent' | 'PartialHelp' | 'Dependent';
export type ObservationCategory = 'Care' | 'Behaviour' | 'Nutrition' | 'Medical' | 'Other';
export type CareTaskStatus = 'Todo' | 'Done';

export const AUTONOMY_LEVEL_LABELS: Record<AutonomyLevel, string> = {
  Independent: 'Autonome',
  PartialHelp: 'Aide partielle',
  Dependent: 'Dépendant',
};
export const AUTONOMY_LEVELS: AutonomyLevel[] = ['Independent', 'PartialHelp', 'Dependent'];

export const OBSERVATION_CATEGORY_LABELS: Record<ObservationCategory, string> = {
  Care: 'Soin',
  Behaviour: 'Comportement',
  Nutrition: 'Alimentation',
  Medical: 'Médical',
  Other: 'Autre',
};

// Les 5 activités de dépendance (besoins de Henderson) + leurs libellés.
export const DEPENDENCY_ACTIVITIES = ['eating', 'elimination', 'mobility', 'dressing', 'hygiene'] as const;
export type DependencyActivity = (typeof DEPENDENCY_ACTIVITIES)[number];
export const DEPENDENCY_ACTIVITY_LABELS: Record<DependencyActivity, string> = {
  eating: 'Boire et manger',
  elimination: 'Éliminer',
  mobility: 'Se mouvoir',
  dressing: 'Se vêtir',
  hygiene: 'Être propre',
};

// Risques surveillés (clé = nom du champ, pour piloter le formulaire).
export const RESIDENT_RISKS = [
  { key: 'fallRisk', label: 'Risque de chute' },
  { key: 'pressureSoreRisk', label: "Risque d'escarre" },
  { key: 'malnutritionRisk', label: 'Risque de dénutrition' },
] as const;

export type RiskLevel = 'None' | 'Low' | 'Moderate' | 'High';
export const RISK_LEVELS: RiskLevel[] = ['None', 'Low', 'Moderate', 'High'];
export const RISK_LEVEL_LABELS: Record<RiskLevel, string> = {
  None: 'Aucun',
  Low: 'Faible',
  Moderate: 'Modéré',
  High: 'Élevé',
};

export type DependencyDto = Record<DependencyActivity, AutonomyLevel>;

// ---- DTOs de lecture ----

export interface ResidentSummary {
  id: string;
  firstName: string;
  lastName: string;
  roomNumber: string;
  overallDependency: AutonomyLevel;
  fallRisk: RiskLevel;
  isArchived: boolean;
}

export interface ObservationDto {
  id: string;
  category: ObservationCategory;
  content: string;
  createdAt: string;
  caregiverId: string;
}

export interface CareTaskDto {
  id: string;
  label: string;
  scheduledDate: string;
  status: CareTaskStatus;
  completedByCaregiverId: string | null;
  completedAt: string | null;
}

export interface ResidentDashboard {
  id: string;
  firstName: string;
  lastName: string;
  birthDate: string;
  admissionDate: string;
  roomNumber: string;
  dependency: DependencyDto;
  referentCaregiverId: string;
  isArchived: boolean;
  fallRisk: RiskLevel;
  pressureSoreRisk: RiskLevel;
  malnutritionRisk: RiskLevel;
  attendingPhysician: string | null;
  emergencyContactName: string | null;
  emergencyContactPhone: string | null;
  occupation: string | null;
  interests: string | null;
  family: string | null;
  recentObservations: ObservationDto[];
  todayTasks: CareTaskDto[];
}

// ---- Payloads d'écriture ----

export interface CreateResidentRequest {
  firstName: string;
  lastName: string;
  birthDate: string;
  admissionDate: string;
  roomNumber: string;
  eating: AutonomyLevel;
  elimination: AutonomyLevel;
  mobility: AutonomyLevel;
  dressing: AutonomyLevel;
  hygiene: AutonomyLevel;
  fallRisk: RiskLevel;
  pressureSoreRisk: RiskLevel;
  malnutritionRisk: RiskLevel;
  attendingPhysician: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
  occupation: string;
  interests: string;
  family: string;
  referentCaregiverId: string;
}

// L'API n'autorise pas la modification des dates -> on les retire.
export type UpdateResidentRequest = Omit<CreateResidentRequest, 'birthDate' | 'admissionDate'>;

// L'auteur n'est plus envoyé : l'API le déduit du jeton (utilisateur connecté).
export interface AddObservationRequest {
  category: ObservationCategory;
  content: string;
}
