import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthenticatedUser, LoginRequest, LoginResponse } from '../models/auth';

const TOKEN_KEY = 'resicare.token';
const USER_KEY = 'resicare.user';

/**
 * État d'authentification de l'application, en SIGNALS. Le jeton et l'utilisateur sont
 * persistés dans localStorage pour survivre à un rafraîchissement de page. On ne re-vérifie
 * pas le jeton ici : c'est le serveur qui le valide à chaque appel (JWT = sans état) ; si le
 * jeton est expiré, l'intercepteur reçoit un 401 et déconnecte.
 */
@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);

  private readonly _user = signal<AuthenticatedUser | null>(this.restoreUser());
  private _token: string | null = localStorage.getItem(TOKEN_KEY);

  /** Utilisateur connecté (ou null). Lecture seule pour les composants. */
  readonly user = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isManager = computed(() => this._user()?.role === 'Manager');

  get token(): string | null {
    return this._token;
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>('/api/auth/login', credentials)
      .pipe(tap((response) => this.setSession(response)));
  }

  logout(): void {
    this._token = null;
    this._user.set(null);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  private setSession(response: LoginResponse): void {
    this._token = response.token;
    this._user.set(response.user);
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
  }

  private restoreUser(): AuthenticatedUser | null {
    const raw = localStorage.getItem(USER_KEY);
    return raw ? (JSON.parse(raw) as AuthenticatedUser) : null;
  }
}
