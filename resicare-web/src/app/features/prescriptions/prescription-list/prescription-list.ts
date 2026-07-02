import { Component, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { PrescriptionService } from '../prescription.service';
import { AuthService } from '../../../core/auth/auth.service';
import {
  AdministrationDto,
  ADMINISTRATION_STATUS_LABELS,
  AdministrationStatus,
  MEDICATION_ROUTE_LABELS,
  MEDICATION_ROUTES,
  MedicationRoute,
  PrescriptionDto,
} from '../../../core/models/prescription';

@Component({
  selector: 'app-prescription-list',
  imports: [
    DatePipe,
    RouterLink,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
  ],
  templateUrl: './prescription-list.html',
  styleUrl: './prescription-list.scss',
})
export class PrescriptionList {
  private readonly route = inject(ActivatedRoute);
  private readonly service = inject(PrescriptionService);
  private readonly auth = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly isManager = this.auth.isManager;
  readonly residentId = this.route.snapshot.paramMap.get('id') ?? '';

  readonly prescriptions = signal<PrescriptionDto[]>([]);
  readonly administrations = signal<AdministrationDto[]>([]);
  readonly error = signal<string | null>(null);

  readonly routes = MEDICATION_ROUTES;
  readonly routeLabels = MEDICATION_ROUTE_LABELS;
  readonly statusLabels = ADMINISTRATION_STATUS_LABELS;

  readonly form = this.fb.nonNullable.group({
    medicationName: ['', Validators.required],
    dosage: ['', Validators.required],
    posology: ['', Validators.required],
    route: ['Oral' as MedicationRoute, Validators.required],
    startDate: [new Date().toISOString().slice(0, 10), Validators.required],
    instructions: [''],
  });

  constructor() {
    this.reload();
  }

  prescribe(): void {
    if (this.form.invalid) return;
    this.error.set(null);

    const v = this.form.getRawValue();
    this.service
      .prescribe(this.residentId, {
        medicationName: v.medicationName,
        dosage: v.dosage,
        posology: v.posology,
        route: v.route,
        startDate: v.startDate,
        endDate: null,
        instructions: v.instructions || null,
      })
      .subscribe({
        next: () => {
          this.form.patchValue({ medicationName: '', dosage: '', posology: '', instructions: '' });
          this.reload();
        },
        error: () => this.error.set('Prescription impossible.'),
      });
  }

  discontinue(id: string): void {
    this.service.discontinue(id).subscribe({ next: () => this.reload(), error: () => this.reload() });
  }

  administer(prescriptionId: string, status: AdministrationStatus): void {
    this.error.set(null);
    this.service.recordAdministration(prescriptionId, { status, notes: null }).subscribe({
      next: () => this.reload(),
      error: () => this.error.set("Impossible d'enregistrer (prescription arrêtée ?)."),
    });
  }

  private reload(): void {
    this.service.getByResident(this.residentId).subscribe((list) => this.prescriptions.set(list));
    this.service
      .getAdministrations(this.residentId, new Date().toISOString().slice(0, 10))
      .subscribe((list) => this.administrations.set(list));
  }
}
