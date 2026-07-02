import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AssignmentService } from '../assignment.service';
import { ResidentService } from '../../residents/resident.service';
import { CaregiverService } from '../../caregivers/caregiver.service';
import { AssignmentDetailDto, SHIFT_LABELS, SHIFTS, Shift } from '../../../core/models/assignment';
import { ResidentSummary } from '../../../core/models/resident';
import { CaregiverDto } from '../../../core/models/caregiver';

@Component({
  selector: 'app-planning',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './planning.html',
  styleUrl: './planning.scss',
})
export class Planning {
  private readonly service = inject(AssignmentService);
  private readonly residentsApi = inject(ResidentService);
  private readonly caregiversApi = inject(CaregiverService);
  private readonly fb = inject(FormBuilder);

  readonly assignments = signal<AssignmentDetailDto[]>([]);
  readonly residents = signal<ResidentSummary[]>([]);
  readonly caregivers = signal<CaregiverDto[]>([]);
  readonly error = signal<string | null>(null);

  readonly shifts = SHIFTS;
  readonly shiftLabels = SHIFT_LABELS;
  readonly date = signal(this.todayIso());

  readonly form = this.fb.nonNullable.group({
    residentId: ['', Validators.required],
    caregiverId: ['', Validators.required],
    shift: ['Morning' as Shift, Validators.required],
  });

  constructor() {
    this.residentsApi.getAll().subscribe((list) => this.residents.set(list));
    this.caregiversApi.getAll().subscribe((list) => this.caregivers.set(list));
    this.load();
  }

  onDateChange(value: string): void {
    this.date.set(value || this.todayIso());
    this.load();
  }

  assign(): void {
    if (this.form.invalid) return;
    this.error.set(null);

    this.service.create({ ...this.form.getRawValue(), date: this.date() }).subscribe({
      next: () => {
        this.form.patchValue({ residentId: '', caregiverId: '' });
        this.load();
      },
      error: () =>
        this.error.set('Affectation impossible (ce résident est-il déjà affecté pour ce poste ?).'),
    });
  }

  remove(id: string): void {
    this.service.remove(id).subscribe(() => this.load());
  }

  private load(): void {
    this.service.getByDate(this.date()).subscribe((list) => this.assignments.set(list));
  }

  private todayIso(): string {
    return new Date().toISOString().slice(0, 10);
  }
}
