import { Routes } from '@angular/router';
import { ResidentList } from './features/residents/resident-list/resident-list';
import { authGuard, managerGuard } from './core/auth/auth.guard';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'residents' },

  // Page de connexion : accessible sans jeton (pas de garde).
  {
    path: 'login',
    title: 'Connexion — ResiCare',
    loadComponent: () => import('./features/auth/login/login').then((m) => m.Login),
  },

  // Tout le reste exige une session (authGuard) ; certaines routes exigent le rôle responsable.
  { path: 'residents', component: ResidentList, title: 'Résidents — ResiCare', canActivate: [authGuard] },

  {
    path: 'pointage',
    title: 'Pointage — ResiCare',
    canActivate: [authGuard],
    loadComponent: () => import('./features/time-clock/time-clock').then((m) => m.TimeClock),
  },

  {
    path: 'planning',
    title: 'Planning — ResiCare',
    canActivate: [authGuard, managerGuard],
    loadComponent: () => import('./features/assignments/planning/planning').then((m) => m.Planning),
  },

  // 'new' AVANT ':id', sinon "new" serait interprété comme un identifiant.
  // loadComponent = lazy loading : ces écrans sont chargés à la demande (bundle séparé).
  {
    path: 'residents/new',
    title: 'Nouveau résident — ResiCare',
    canActivate: [authGuard, managerGuard],
    loadComponent: () =>
      import('./features/residents/resident-form/resident-form').then((m) => m.ResidentForm),
  },
  {
    path: 'residents/:id/edit',
    title: 'Modifier — ResiCare',
    canActivate: [authGuard, managerGuard],
    loadComponent: () =>
      import('./features/residents/resident-form/resident-form').then((m) => m.ResidentForm),
  },
  {
    path: 'residents/:id/observations',
    title: 'Observations — ResiCare',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/observations/observation-list/observation-list').then((m) => m.ObservationList),
  },
  {
    path: 'residents/:id/prescriptions',
    title: 'Traitement — ResiCare',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/prescriptions/prescription-list/prescription-list').then((m) => m.PrescriptionList),
  },
  {
    path: 'residents/:id',
    title: 'Tableau de bord — ResiCare',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/residents/resident-dashboard/resident-dashboard').then(
        (m) => m.ResidentDashboard,
      ),
  },
];
