import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';

import { Login } from './login';
import { AuthService } from '../../../core/auth/auth.service';
import { LoginResponse } from '../../../core/models/auth';

const loginResponse: LoginResponse = {
  token: 't',
  expiresAtUtc: '2026-06-24T00:00:00Z',
  user: {
    id: '1',
    firstName: 'Paul',
    lastName: 'Durand',
    email: 'paul.durand@resicare.local',
    role: 'Caregiver',
  },
};

function configure(login: ReturnType<typeof vi.fn>) {
  const router = { navigateByUrl: vi.fn() };
  const route = { snapshot: { queryParamMap: { get: () => null } } };

  TestBed.configureTestingModule({
    imports: [Login],
    providers: [
      { provide: AuthService, useValue: { login } },
      { provide: Router, useValue: router },
      { provide: ActivatedRoute, useValue: route },
    ],
  });

  return router;
}

describe('Login', () => {
  it('se connecte puis navigue vers la cible (par défaut /residents)', () => {
    const login = vi.fn().mockReturnValue(of(loginResponse));
    const router = configure(login);

    const component = TestBed.createComponent(Login).componentInstance;
    component.form.setValue({ email: 'paul.durand@resicare.local', password: 'Soignant123!' });
    component.submit();

    expect(login).toHaveBeenCalledWith({
      email: 'paul.durand@resicare.local',
      password: 'Soignant123!',
    });
    expect(router.navigateByUrl).toHaveBeenCalledWith('/residents');
  });

  it('affiche une erreur quand la connexion échoue', () => {
    const login = vi.fn().mockReturnValue(throwError(() => new Error('401')));
    configure(login);

    const component = TestBed.createComponent(Login).componentInstance;
    component.form.setValue({ email: 'x@y.z', password: 'mauvais' });
    component.submit();

    expect(component.error()).not.toBeNull();
  });

  it('ne soumet rien si le formulaire est invalide', () => {
    const login = vi.fn().mockReturnValue(of(loginResponse));
    configure(login);

    const component = TestBed.createComponent(Login).componentInstance;
    component.form.setValue({ email: '', password: '' });
    component.submit();

    expect(login).not.toHaveBeenCalled();
  });
});
