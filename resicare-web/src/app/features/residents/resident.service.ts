import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  CreateResidentRequest,
  ResidentDashboard,
  ResidentSummary,
  UpdateResidentRequest,
} from '../../core/models/resident';

/**
 * Accès HTTP aux résidents. Les URL sont relatives (/api/...) : en dev, le proxy
 * Angular les transfère à l'API .NET (http://localhost:5045) sans souci CORS.
 */
@Injectable({ providedIn: 'root' })
export class ResidentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/residents';

  getAll(search?: string): Observable<ResidentSummary[]> {
    const params = search ? new HttpParams().set('search', search) : undefined;
    return this.http.get<ResidentSummary[]>(this.baseUrl, { params });
  }

  getById(id: string): Observable<ResidentDashboard> {
    return this.http.get<ResidentDashboard>(`${this.baseUrl}/${id}`);
  }

  create(body: CreateResidentRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.baseUrl, body);
  }

  update(id: string, body: UpdateResidentRequest): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, body);
  }

  archive(id: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/${id}/archive`, {});
  }

  // Le PDF est protégé : on le télécharge via HttpClient (l'intercepteur ajoute le jeton),
  // un simple <a href> ne transmettrait pas l'en-tête Authorization.
  getPdf(id: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/${id}/pdf`, { responseType: 'blob' });
  }
}
