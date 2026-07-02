import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  AssignmentDetailDto,
  AssignmentResidentDto,
  CreateAssignmentRequest,
} from '../../core/models/assignment';

@Injectable({ providedIn: 'root' })
export class AssignmentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/assignments';

  /** Planning complet d'une date — réservé au responsable (403 sinon). */
  getByDate(date: string): Observable<AssignmentDetailDto[]> {
    return this.http.get<AssignmentDetailDto[]>(this.baseUrl, {
      params: new HttpParams().set('date', date),
    });
  }

  /** Mes résidents du jour (l'identité vient du jeton). */
  getMine(date: string): Observable<AssignmentResidentDto[]> {
    return this.http.get<AssignmentResidentDto[]>(`${this.baseUrl}/me`, {
      params: new HttpParams().set('date', date),
    });
  }

  create(body: CreateAssignmentRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.baseUrl, body);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
