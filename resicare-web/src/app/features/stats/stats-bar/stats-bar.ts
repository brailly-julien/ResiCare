import { Component, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatCardModule } from '@angular/material/card';

import { StatsService } from '../stats.service';
import { StatsDto } from '../../../core/models/stats';

@Component({
  selector: 'app-stats-bar',
  imports: [MatCardModule],
  templateUrl: './stats-bar.html',
  styleUrl: './stats-bar.scss',
})
export class StatsBar {
  private readonly service = inject(StatsService);

  readonly stats = signal<StatsDto | null>(null);

  // Taux de tâches réalisées = signal calculé à partir des stats.
  readonly completionRate = computed(() => {
    const s = this.stats();
    if (!s || s.tasksToday === 0) return 0;
    return Math.round((s.tasksDoneToday / s.tasksToday) * 100);
  });

  constructor() {
    this.service
      .getStats()
      .pipe(takeUntilDestroyed())
      .subscribe((s) => this.stats.set(s));
  }
}
