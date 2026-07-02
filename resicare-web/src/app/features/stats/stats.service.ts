import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StatsDto } from '../../core/models/stats';

@Injectable({ providedIn: 'root' })
export class StatsService {
  private readonly http = inject(HttpClient);

  getStats(): Observable<StatsDto> {
    return this.http.get<StatsDto>('/api/stats');
  }
}
