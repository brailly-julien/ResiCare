import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CaregiverDto } from '../../core/models/caregiver';

@Injectable({ providedIn: 'root' })
export class CaregiverService {
  private readonly http = inject(HttpClient);

  getAll(): Observable<CaregiverDto[]> {
    return this.http.get<CaregiverDto[]>('/api/caregivers');
  }
}
