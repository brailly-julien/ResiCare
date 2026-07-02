import { Component, inject, signal } from '@angular/core';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { catchError, debounceTime, distinctUntilChanged, of, startWith, switchMap, tap } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatButtonModule } from '@angular/material/button';

import { ResidentService } from '../resident.service';
import { StatsBar } from '../../stats/stats-bar/stats-bar';
import { AuthService } from '../../../core/auth/auth.service';
import { AssignmentService } from '../../assignments/assignment.service';
import { AssignmentResidentDto, SHIFT_LABELS } from '../../../core/models/assignment';
import { AUTONOMY_LEVEL_LABELS, AutonomyLevel, ResidentSummary } from '../../../core/models/resident';

@Component({
  selector: 'app-resident-list',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressBarModule,
    MatButtonModule,
    StatsBar,
  ],
  templateUrl: './resident-list.html',
  styleUrl: './resident-list.scss',
})
export class ResidentList {
  private readonly service = inject(ResidentService);
  private readonly auth = inject(AuthService);
  private readonly assignmentsApi = inject(AssignmentService);

  readonly isManager = this.auth.isManager;
  readonly myAssignments = signal<AssignmentResidentDto[]>([]);
  readonly shiftLabels = SHIFT_LABELS;
  readonly searchControl = new FormControl('', { nonNullable: true });
  readonly residents = signal<ResidentSummary[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  constructor() {
    this.searchControl.valueChanges
      .pipe(
        startWith(''),
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => {
          this.loading.set(true);
          this.error.set(null);
        }),
        switchMap((term) =>
          this.service.getAll(term).pipe(
            catchError(() => {
              this.error.set("Impossible de charger les résidents. L'API est-elle démarrée ?");
              return of<ResidentSummary[]>([]);
            }),
          ),
        ),
        tap(() => this.loading.set(false)),
        takeUntilDestroyed(),
      )
      .subscribe((list) => this.residents.set(list));

    // « Mes résidents du jour » : les affectations du soignant connecté pour aujourd'hui.
    this.assignmentsApi
      .getMine(new Date().toISOString().slice(0, 10))
      .pipe(takeUntilDestroyed())
      .subscribe((list) => this.myAssignments.set(list));
  }

  levelLabel(level: AutonomyLevel): string {
    return AUTONOMY_LEVEL_LABELS[level];
  }
}
