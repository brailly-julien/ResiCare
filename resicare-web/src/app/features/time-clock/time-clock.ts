import { Component, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';

import { TimeClockService } from './time-clock.service';
import { AuthService } from '../../core/auth/auth.service';
import { CaregiverTimeEntryDto, TimeEntryDto } from '../../core/models/time-entry';

@Component({
  selector: 'app-time-clock',
  imports: [DatePipe, MatCardModule, MatButtonModule, MatIconModule, MatProgressBarModule],
  templateUrl: './time-clock.html',
  styleUrl: './time-clock.scss',
})
export class TimeClock {
  private readonly service = inject(TimeClockService);
  private readonly auth = inject(AuthService);

  readonly isManager = this.auth.isManager;
  readonly myEntries = signal<TimeEntryDto[]>([]);
  readonly presence = signal<CaregiverTimeEntryDto[]>([]);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  // Pointage en cours = première entrée sans heure de départ.
  readonly activeShift = computed(() => this.myEntries().find((e) => e.clockOutAt === null) ?? null);

  constructor() {
    this.reloadMine();
    if (this.isManager()) this.reloadPresence();
  }

  clockIn(): void {
    this.error.set(null);
    this.service.clockIn().subscribe({
      next: () => this.refresh(),
      error: () => this.error.set('Impossible de pointer (un pointage est-il déjà en cours ?).'),
    });
  }

  clockOut(): void {
    this.error.set(null);
    this.service.clockOut().subscribe({
      next: () => this.refresh(),
      error: () => this.error.set('Impossible de pointer le départ.'),
    });
  }

  formatDuration(minutes: number | null): string {
    if (minutes === null) return '—';
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    return h > 0 ? `${h} h ${m.toString().padStart(2, '0')}` : `${m} min`;
  }

  caregiverName(e: CaregiverTimeEntryDto): string {
    return `${e.caregiverLastName} ${e.caregiverFirstName}`;
  }

  private refresh(): void {
    this.reloadMine();
    if (this.isManager()) this.reloadPresence();
  }

  private reloadMine(): void {
    this.loading.set(true);
    this.service.getMine().subscribe({
      next: (list) => {
        this.myEntries.set(list);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  private reloadPresence(): void {
    this.service.getPresence().subscribe((list) => this.presence.set(list));
  }
}
