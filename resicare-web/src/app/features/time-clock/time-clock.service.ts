import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CaregiverTimeEntryDto, TimeEntryDto } from '../../core/models/time-entry';

@Injectable({ providedIn: 'root' })
export class TimeClockService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/timeclock';

  clockIn(): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${this.baseUrl}/clock-in`, {});
  }

  clockOut(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/clock-out`, {});
  }

  /** Mes pointages d'aujourd'hui (l'identité vient du jeton côté serveur). */
  getMine(): Observable<TimeEntryDto[]> {
    return this.http.get<TimeEntryDto[]>(`${this.baseUrl}/me`);
  }

  /** Présences du jour — réservé au responsable (403 sinon). */
  getPresence(): Observable<CaregiverTimeEntryDto[]> {
    return this.http.get<CaregiverTimeEntryDto[]>(this.baseUrl);
  }
}
