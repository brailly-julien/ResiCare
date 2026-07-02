import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AdministrationDto,
  PrescribeMedicationRequest,
  PrescriptionDto,
  RecordAdministrationRequest,
} from '../../core/models/prescription';

@Injectable({ providedIn: 'root' })
export class PrescriptionService {
  private readonly http = inject(HttpClient);

  getByResident(residentId: string): Observable<PrescriptionDto[]> {
    return this.http.get<PrescriptionDto[]>(`/api/residents/${residentId}/prescriptions`);
  }

  getAdministrations(residentId: string, date: string): Observable<AdministrationDto[]> {
    return this.http.get<AdministrationDto[]>(`/api/residents/${residentId}/administrations`, {
      params: new HttpParams().set('date', date),
    });
  }

  prescribe(residentId: string, body: PrescribeMedicationRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`/api/residents/${residentId}/prescriptions`, body);
  }

  discontinue(id: string): Observable<void> {
    return this.http.post<void>(`/api/prescriptions/${id}/discontinue`, {});
  }

  recordAdministration(id: string, body: RecordAdministrationRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`/api/prescriptions/${id}/administrations`, body);
  }
}
