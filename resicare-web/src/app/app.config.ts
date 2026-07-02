import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { authInterceptor } from './core/auth/auth.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    // Client HTTP (API Fetch, idéal en zoneless) + intercepteur qui attache le jeton JWT
    // à chaque requête et déconnecte sur 401.
    provideHttpClient(withFetch(), withInterceptors([authInterceptor])),
  ],
};
