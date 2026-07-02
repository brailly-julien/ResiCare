import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';

import { ObservationService } from '../observation.service';
import { CaregiverService } from '../../caregivers/caregiver.service';
import {
  OBSERVATION_CATEGORY_LABELS,
  ObservationCategory,
  ObservationDto,
} from '../../../core/models/resident';
import { CaregiverDto } from '../../../core/models/caregiver';

@Component({
  selector: 'app-observation-list',
  imports: [
    DatePipe,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatSelectModule,
    MatPaginatorModule,
  ],
  templateUrl: './observation-list.html',
  styleUrl: './observation-list.scss',
})
export class ObservationList {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(ObservationService);
  private readonly caregiversApi = inject(CaregiverService);

  readonly observations = signal<ObservationDto[]>([]);
  readonly totalCount = signal(0);
  readonly caregivers = signal<CaregiverDto[]>([]);

  readonly categories: ObservationCategory[] = ['Care', 'Behaviour', 'Nutrition', 'Medical', 'Other'];
  readonly categoryLabels = OBSERVATION_CATEGORY_LABELS;

  readonly pageSize = 5;
  pageIndex = 0;
  category: ObservationCategory | null = null;

  protected residentId = '';

  constructor() {
    this.caregiversApi.getAll().pipe(takeUntilDestroyed()).subscribe((list) => this.caregivers.set(list));
    this.residentId = this.route.snapshot.paramMap.get('id') ?? '';
    this.load();
  }

  load(): void {
    this.service
      .getByResident(this.residentId, this.pageIndex + 1, this.pageSize, this.category ?? undefined)
      .subscribe((result) => {
        this.observations.set(result.items);
        this.totalCount.set(result.totalCount);
      });
  }

  // mat-paginator est 0-based ; l'API est 1-based.
  onPage(event: PageEvent): void {
    this.pageIndex = event.pageIndex;
    this.load();
  }

  onCategoryChange(value: ObservationCategory | null): void {
    this.category = value;
    this.pageIndex = 0;
    this.load();
  }

  caregiverName(id: string): string {
    const c = this.caregivers().find((x) => x.id === id);
    return c ? `${c.lastName} ${c.firstName}` : '—';
  }
}
