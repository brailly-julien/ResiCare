import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

import { ResidentService } from '../resident.service';
import { CaregiverService } from '../../caregivers/caregiver.service';
import {
  AUTONOMY_LEVEL_LABELS,
  AUTONOMY_LEVELS,
  AutonomyLevel,
  DEPENDENCY_ACTIVITIES,
  DEPENDENCY_ACTIVITY_LABELS,
  RESIDENT_RISKS,
  RISK_LEVEL_LABELS,
  RISK_LEVELS,
  RiskLevel,
  UpdateResidentRequest,
} from '../../../core/models/resident';
import { CaregiverDto } from '../../../core/models/caregiver';

@Component({
  selector: 'app-resident-form',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
  ],
  templateUrl: './resident-form.html',
  styleUrl: './resident-form.scss',
})
export class ResidentForm {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly residents = inject(ResidentService);
  private readonly caregiversApi = inject(CaregiverService);

  readonly caregivers = signal<CaregiverDto[]>([]);
  readonly isEdit = signal(false);
  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly autonomyLevels = AUTONOMY_LEVELS;
  readonly autonomyLabels = AUTONOMY_LEVEL_LABELS;
  readonly activities = DEPENDENCY_ACTIVITIES;
  readonly activityLabels = DEPENDENCY_ACTIVITY_LABELS;
  readonly risks = RESIDENT_RISKS;
  readonly riskLevels = RISK_LEVELS;
  readonly riskLevelLabels = RISK_LEVEL_LABELS;

  readonly form = new FormGroup({
    firstName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    lastName: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(100)] }),
    birthDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    admissionDate: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
    roomNumber: new FormControl('', { nonNullable: true, validators: [Validators.required, Validators.maxLength(20)] }),
    eating: new FormControl<AutonomyLevel>('Independent', { nonNullable: true }),
    elimination: new FormControl<AutonomyLevel>('Independent', { nonNullable: true }),
    mobility: new FormControl<AutonomyLevel>('Independent', { nonNullable: true }),
    dressing: new FormControl<AutonomyLevel>('Independent', { nonNullable: true }),
    hygiene: new FormControl<AutonomyLevel>('Independent', { nonNullable: true }),
    fallRisk: new FormControl<RiskLevel>('None', { nonNullable: true }),
    pressureSoreRisk: new FormControl<RiskLevel>('None', { nonNullable: true }),
    malnutritionRisk: new FormControl<RiskLevel>('None', { nonNullable: true }),
    attendingPhysician: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(200)] }),
    emergencyContactName: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(200)] }),
    emergencyContactPhone: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(30)] }),
    occupation: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(200)] }),
    interests: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    family: new FormControl('', { nonNullable: true, validators: [Validators.maxLength(1000)] }),
    referentCaregiverId: new FormControl('', { nonNullable: true, validators: [Validators.required] }),
  });

  private residentId: string | null = null;

  constructor() {
    this.caregiversApi.getAll().pipe(takeUntilDestroyed()).subscribe((list) => this.caregivers.set(list));

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit.set(true);
      this.residentId = id;
      this.form.controls.birthDate.disable();
      this.form.controls.admissionDate.disable();

      this.residents.getById(id).subscribe((d) =>
        this.form.patchValue({
          firstName: d.firstName,
          lastName: d.lastName,
          birthDate: d.birthDate,
          admissionDate: d.admissionDate,
          roomNumber: d.roomNumber,
          eating: d.dependency.eating,
          elimination: d.dependency.elimination,
          mobility: d.dependency.mobility,
          dressing: d.dependency.dressing,
          hygiene: d.dependency.hygiene,
          fallRisk: d.fallRisk,
          pressureSoreRisk: d.pressureSoreRisk,
          malnutritionRisk: d.malnutritionRisk,
          attendingPhysician: d.attendingPhysician ?? '',
          emergencyContactName: d.emergencyContactName ?? '',
          emergencyContactPhone: d.emergencyContactPhone ?? '',
          occupation: d.occupation ?? '',
          interests: d.interests ?? '',
          family: d.family ?? '',
          referentCaregiverId: d.referentCaregiverId,
        }),
      );
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.saving.set(true);
    this.error.set(null);
    const v = this.form.getRawValue();

    const common: UpdateResidentRequest = {
      firstName: v.firstName,
      lastName: v.lastName,
      roomNumber: v.roomNumber,
      eating: v.eating,
      elimination: v.elimination,
      mobility: v.mobility,
      dressing: v.dressing,
      hygiene: v.hygiene,
      fallRisk: v.fallRisk,
      pressureSoreRisk: v.pressureSoreRisk,
      malnutritionRisk: v.malnutritionRisk,
      attendingPhysician: v.attendingPhysician,
      emergencyContactName: v.emergencyContactName,
      emergencyContactPhone: v.emergencyContactPhone,
      occupation: v.occupation,
      interests: v.interests,
      family: v.family,
      referentCaregiverId: v.referentCaregiverId,
    };

    if (this.isEdit() && this.residentId) {
      this.residents.update(this.residentId, common).subscribe({
        next: () => this.router.navigate(['/residents', this.residentId]),
        error: () => {
          this.error.set("Échec de l'enregistrement.");
          this.saving.set(false);
        },
      });
    } else {
      this.residents.create({ ...common, birthDate: v.birthDate, admissionDate: v.admissionDate }).subscribe({
        next: (res) => this.router.navigate(['/residents', res.id]),
        error: () => {
          this.error.set('Échec de la création.');
          this.saving.set(false);
        },
      });
    }
  }
}
