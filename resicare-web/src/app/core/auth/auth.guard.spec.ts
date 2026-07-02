import { TestBed } from '@angular/core/testing';
import { ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, provideRouter } from '@angular/router';

import { authGuard, managerGuard } from './auth.guard';
import { AuthService } from './auth.service';

function setup(state: { authenticated?: boolean; manager?: boolean }): void {
  const auth = {
    isAuthenticated: () => state.authenticated ?? false,
    isManager: () => state.manager ?? false,
  } as unknown as AuthService;

  TestBed.configureTestingModule({
    providers: [provideRouter([]), { provide: AuthService, useValue: auth }],
  });
}

const route = {} as ActivatedRouteSnapshot;
const routerState = { url: '/residents' } as RouterStateSnapshot;

describe('authGuard', () => {
  it('laisse passer un utilisateur authentifié', () => {
    setup({ authenticated: true });

    const result = TestBed.runInInjectionContext(() => authGuard(route, routerState));

    expect(result).toBe(true);
  });

  it('redirige vers /login (avec returnUrl) sinon', () => {
    setup({ authenticated: false });

    const result = TestBed.runInInjectionContext(() => authGuard(route, routerState));

    expect(result).toBeInstanceOf(UrlTree);
    expect((result as UrlTree).toString()).toContain('/login');
    expect((result as UrlTree).toString()).toContain('returnUrl');
  });
});

describe('managerGuard', () => {
  it('laisse passer un responsable', () => {
    setup({ authenticated: true, manager: true });

    const result = TestBed.runInInjectionContext(() => managerGuard(route, routerState));

    expect(result).toBe(true);
  });

  it('renvoie un soignant vers /residents', () => {
    setup({ authenticated: true, manager: false });

    const result = TestBed.runInInjectionContext(() => managerGuard(route, routerState));

    expect(result).toBeInstanceOf(UrlTree);
    expect((result as UrlTree).toString()).toContain('/residents');
  });
});
