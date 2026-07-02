import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CareTaskDto } from '../../core/models/resident';

@Injectable({ providedIn: 'root' })
export class CareTaskService {
  private readonly http = inject(HttpClient);

  getByResidentAndDate(residentId: string, date?: string): Observable<CareTaskDto[]> {
    const url = date
      ? `/api/residents/${residentId}/tasks?date=${date}`
      : `/api/residents/${residentId}/tasks`;
    return this.http.get<CareTaskDto[]>(url);
  }

  complete(taskId: string): Observable<void> {
    // Le soignant qui valide est l'utilisateur connecté (jeton) : corps vide.
    return this.http.post<void>(`/api/tasks/${taskId}/complete`, {});
  }
}
