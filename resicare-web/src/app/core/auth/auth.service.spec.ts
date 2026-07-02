import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';

import { AuthService } from './auth.service';
import { LoginResponse } from '../models/auth';

describe('AuthService', () => {
  const managerResponse: LoginResponse = {
    token: 'jwt-token-abc',
    expiresAtUtc: '2026-06-24T12:00:00Z',
    user: {
      id: '11111111-1111-1111-1111-111111111111',
      firstName: 'Marie',
      lastName: 'Curie',
      email: 'marie.curie@resicare.local',
      role: 'Manager',
    },
  };

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
  });

  it('démarre déconnecté', () => {
    const auth = TestBed.inject(AuthService);

    expect(auth.isAuthenticated()).toBe(false);
    expect(auth.user()).toBeNull();
    expect(auth.token).toBeNull();
  });

  it("login() stocke le jeton, l'utilisateur, et expose isManager", () => {
    const auth = TestBed.inject(AuthService);
    const httpMock = TestBed.inject(HttpTestingController);

    auth.login({ email: 'marie.curie@resicare.local', password: 'Manager123!' }).subscribe();

    const req = httpMock.expectOne('/api/auth/login');
    expect(req.request.method).toBe('POST');
    req.flush(managerResponse);

    expect(auth.isAuthenticated()).toBe(true);
    expect(auth.isManager()).toBe(true);
    expect(auth.token).toBe('jwt-token-abc');
    expect(auth.user()?.lastName).toBe('Curie');
    expect(localStorage.getItem('resicare.token')).toBe('jwt-token-abc');

    httpMock.verify();
  });

  it('logout() efface la session', () => {
    const auth = TestBed.inject(AuthService);
    const httpMock = TestBed.inject(HttpTestingController);

    auth.login({ email: 'marie.curie@resicare.local', password: 'Manager123!' }).subscribe();
    httpMock.expectOne('/api/auth/login').flush(managerResponse);

    auth.logout();

    expect(auth.isAuthenticated()).toBe(false);
    expect(auth.user()).toBeNull();
    expect(localStorage.getItem('resicare.token')).toBeNull();
    httpMock.verify();
  });

  it('restaure la session depuis localStorage au démarrage', () => {
    localStorage.setItem('resicare.token', 'persisted-token');
    localStorage.setItem('resicare.user', JSON.stringify(managerResponse.user));

    const auth = TestBed.inject(AuthService);

    expect(auth.isAuthenticated()).toBe(true);
    expect(auth.isManager()).toBe(true);
    expect(auth.token).toBe('persisted-token');
  });
});
