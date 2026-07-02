import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddObservationRequest, ObservationCategory, ObservationDto } from '../../core/models/resident';
import { PagedResult } from '../../core/models/paged-result';

@Injectable({ providedIn: 'root' })
export class ObservationService {
  private readonly http = inject(HttpClient);

  getByResident(
    residentId: string,
    page: number,
    pageSize: number,
    category?: ObservationCategory,
  ): Observable<PagedResult<ObservationDto>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (category) {
      params = params.set('category', category);
    }
    return this.http.get<PagedResult<ObservationDto>>(
      `/api/residents/${residentId}/observations`,
      { params },
    );
  }

  add(residentId: string, body: AddObservationRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`/api/residents/${residentId}/observations`, body);
  }
}
