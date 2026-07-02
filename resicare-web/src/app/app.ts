import { Component, inject } from '@angular/core';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from './core/auth/auth.service';
import { CAREGIVER_ROLE_LABELS } from './core/models/caregiver';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, MatToolbarModule, MatButtonModule, MatIconModule],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly user = this.auth.user;
  readonly isManager = this.auth.isManager;
  readonly roleLabels = CAREGIVER_ROLE_LABELS;

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }
}
