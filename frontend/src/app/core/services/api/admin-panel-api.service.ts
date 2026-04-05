import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { unwrapApiResponse } from '../../http/api-response.mapper';

@Injectable({ providedIn: 'root' })
export class AdminPanelApiService {
  private readonly base = '/api';

  constructor(private readonly http: HttpClient) {}

  getAdminLogs(): Observable<{ logs: string[]; filename: string }> {
    return this.http.get<unknown>(`${this.base}/AdminPanel/logs`).pipe(
      map(res => {
        const d = unwrapApiResponse<Record<string, unknown>>(res) ?? {};
        const rawLogs = d['logs'] ?? d['Logs'];
        const logs = Array.isArray(rawLogs) ? rawLogs.map(String) : [];
        const filename = String(d['filename'] ?? d['Filename'] ?? '');
        return { logs, filename };
      })
    );
  }

  getAuditLogs(page = 1, size = 50, entity?: string, action?: string): Observable<unknown> {
    let params = new HttpParams().set('page', page).set('size', size);
    if (entity) {
      params = params.set('entity', entity);
    }
    if (action) {
      params = params.set('action', action);
    }
    return this.http.get<unknown>(`${this.base}/AdminPanel/audit-logs`, { params }).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  getLoginEvents(page = 1, size = 50): Observable<unknown> {
    const params = new HttpParams().set('page', page).set('size', size);
    return this.http.get<unknown>(`${this.base}/AdminPanel/login-events`, { params }).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  getAgreementActions(page = 1, size = 50): Observable<unknown> {
    const params = new HttpParams().set('page', page).set('size', size);
    return this.http.get<unknown>(`${this.base}/AdminPanel/agreement-actions`, { params }).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  getAdminUsers(search?: string): Observable<unknown[]> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    return this.http.get<unknown>(`${this.base}/AdminPanel/users`, { params }).pipe(
      map(res => {
        const unwrapped = unwrapApiResponse<unknown[]>(res);
        if (Array.isArray(unwrapped)) {
          return unwrapped;
        }
        const r = res as { data?: unknown[] };
        return Array.isArray(r.data) ? r.data : Array.isArray(res) ? (res as unknown[]) : [];
      })
    );
  }

  updateAdminUser(id: string, payload: unknown): Observable<unknown> {
    return this.http.put<unknown>(`${this.base}/AdminPanel/users/${id}`, payload).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  resetUserPassword(id: string, newPassword: string): Observable<unknown> {
    return this.http
      .post<unknown>(`${this.base}/AdminPanel/users/${id}/reset-password`, { newPassword })
      .pipe(map(res => unwrapApiResponse(res) ?? res));
  }

  getAdminRoles(): Observable<unknown[]> {
    return this.http.get<unknown>(`${this.base}/AdminPanel/roles`).pipe(
      map(res => {
        const d = unwrapApiResponse<unknown[]>(res);
        if (Array.isArray(d)) {
          return d;
        }
        return Array.isArray(res) ? (res as unknown[]) : [];
      })
    );
  }
}
