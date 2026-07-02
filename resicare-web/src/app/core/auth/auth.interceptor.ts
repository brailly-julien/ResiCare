import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from './auth.service';

/**
 * Intercepteur HTTP fonctionnel : attache « Authorization: Bearer <jeton> » à chaque appel
 * et, en cas de 401 (jeton absent/expiré/refusé), déconnecte et renvoie vers la page de login.
 * Le 403 (authentifié mais sans le rôle) n'est PAS traité ici : on laisse le composant afficher
 * un message — l'utilisateur reste connecté.
 */
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const token = auth.token;

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && !req.url.includes('/api/auth/login')) {
        auth.logout();
        router.navigate(['/login']);
      }
      return throwError(() => error);
    }),
  );
};
