import { CaregiverRole } from './caregiver';

/** Identité de l'utilisateur connecté (miroir de AuthenticatedUserDto). */
export interface AuthenticatedUser {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: CaregiverRole;
}

export interface LoginRequest {
  email: string;
  password: string;
}

/** Réponse de /api/auth/login : le jeton signé, son expiration et l'utilisateur. */
export interface LoginResponse {
  token: string;
  expiresAtUtc: string;
  user: AuthenticatedUser;
}
