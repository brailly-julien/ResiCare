import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { catchError, of, switchMap } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { ResidentService } from '../resident.service';
import { ObservationService } from '../../observations/observation.service';
import { CareTaskService } from '../../care-tasks/care-task.service';
import { CaregiverService } from '../../caregivers/caregiver.service';
import { AuthService } from '../../../core/auth/auth.service';
import {
  AUTONOMY_LEVEL_LABELS,
  DEPENDENCY_ACTIVITIES,
  DEPENDENCY_ACTIVITY_LABELS,
  OBSERVATION_CATEGORY_LABELS,
  ObservationCategory,
  RESIDENT_RISKS,
  RISK_LEVEL_LABELS,
  ResidentDashboard as ResidentDashboardModel,
} from '../../../core/models/resident';
import { CaregiverDto } from '../../../core/models/caregiver';

@Component({
  selector: 'app-resident-dashboard',
  imports: [
    DatePipe,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressBarModule,
  ],
  templateUrl: './resident-dashboard.html',
  styleUrl: './resident-dashboard.scss',
})
export class ResidentDashboard {
  private readonly route = inject(ActivatedRoute);
  private readonly residents = inject(ResidentService);
  private readonly observations = inject(ObservationService);
  private readonly tasks = inject(CareTaskService);
  private readonly caregiversApi = inject(CaregiverService);
  private readonly auth = inject(AuthService);

  readonly dashboard = signal<ResidentDashboardModel | null>(null);
  readonly caregivers = signal<CaregiverDto[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);

  // Le « soignant actif » est désormais l'utilisateur connecté (jeton JWT) ; plus de sélecteur.
  readonly currentUser = this.auth.user;
  readonly isManager = this.auth.isManager;

  readonly observationForm = new FormGroup({
    category: new FormControl<ObservationCategory>('Care', {
      nonNullable: true,
      validators: [Validators.required],
    }),
    content: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.maxLength(2000)],
    }),
  });

  readonly categories: ObservationCategory[] = ['Care', 'Behaviour', 'Nutrition', 'Medical', 'Other'];
  readonly categoryLabels = OBSERVATION_CATEGORY_LABELS;
  readonly activities = DEPENDENCY_ACTIVITIES;
  readonly activityLabels = DEPENDENCY_ACTIVITY_LABELS;
  readonly autonomyLabels = AUTONOMY_LEVEL_LABELS;
  readonly risks = RESIDENT_RISKS;
  readonly riskLevelLabels = RISK_LEVEL_LABELS;

  private residentId = '';

  constructor() {
    this.caregiversApi
      .getAll()
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.caregivers.set(list));

    this.route.paramMap
      .pipe(
        switchMap((params) => {
          this.residentId = params.get('id') ?? '';
          this.loading.set(true);
          this.error.set(null);
          return this.residents.getById(this.residentId).pipe(
            catchError(() => {
              this.error.set('Résident introuvable.');
              return of(null);
            }),
          );
        }),
        takeUntilDestroyed(),
      )
      .subscribe((d) => {
        this.dashboard.set(d);
        this.loading.set(false);
      });
  }

  caregiverName(id: string): string {
    const c = this.caregivers().find((x) => x.id === id);
    return c ? `${c.lastName} ${c.firstName}` : '—';
  }

  private reload(): void {
    this.residents.getById(this.residentId).subscribe((d) => this.dashboard.set(d));
  }

  addObservation(): void {
    if (this.observationForm.invalid) return;

    const { category, content } = this.observationForm.getRawValue();
    this.observations.add(this.residentId, { category, content }).subscribe({
      next: () => {
        this.observationForm.reset({ category: 'Care', content: '' });
        this.reload();
      },
      error: () => this.error.set("Impossible d'ajouter l'observation (résident archivé ?)."),
    });
  }

  completeTask(taskId: string): void {
    this.tasks.complete(taskId).subscribe({
      next: () => this.reload(),
      error: () => this.error.set('Cette tâche est peut-être déjà réalisée.'),
    });
  }

  openPdf(id: string): void {
    this.residents.getPdf(id).subscribe((blob) => {
      const url = URL.createObjectURL(blob);
      window.open(url, '_blank');
      setTimeout(() => URL.revokeObjectURL(url), 30_000);
    });
  }

  archive(): void {
    this.residents.archive(this.residentId).subscribe(() => this.reload());
  }
}
