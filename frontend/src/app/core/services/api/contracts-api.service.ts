import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { unwrapApiResponse } from '../../http/api-response.mapper';

function pagedPayload(res: unknown): Record<string, unknown> {
  const unwrapped = unwrapApiResponse<Record<string, unknown>>(res);
  if (unwrapped && typeof unwrapped === 'object') {
    return unwrapped;
  }
  return typeof res === 'object' && res !== null ? (res as Record<string, unknown>) : {};
}

function readPagedItems(body: Record<string, unknown>): unknown[] {
  const items = body['items'] ?? body['Items'];
  return Array.isArray(items) ? items : [];
}

@Injectable({ providedIn: 'root' })
export class ContractsApiService {
  private readonly base = '/api';

  constructor(private readonly http: HttpClient) {}

  getContracts(activeOnly = true): Observable<unknown[]> {
    let params = new HttpParams().set('page', '1').set('size', '500');
    if (activeOnly) {
      params = params.set('activeOnly', 'true');
    }
    return this.http.get<unknown>(`${this.base}/Contracts`, { params }).pipe(
      map(res => readPagedItems(pagedPayload(res)))
    );
  }

  getAdminContracts(
    search?: string,
    status?: string,
    page = 1,
    size = 20
  ): Observable<{ items: unknown[]; total: number; page: number; size: number }> {
    let params = new HttpParams().set('page', page).set('size', size);
    if (search) {
      params = params.set('search', search);
    }
    if (status) {
      params = params.set('status', status);
    }
    return this.http.get<unknown>(`${this.base}/Contracts`, { params }).pipe(
      map(res => {
        const body = pagedPayload(res);
        const items = readPagedItems(body);
        const total = Number(body['total'] ?? body['Total'] ?? 0);
        const pageNum = Number(body['page'] ?? body['Page'] ?? page);
        const sizeNum = Number(body['size'] ?? body['Size'] ?? size);
        return { items, total, page: pageNum, size: sizeNum };
      })
    );
  }

  getContractById(id: string): Observable<unknown> {
    return this.http.get<unknown>(`${this.base}/Contracts/${id}`).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  getContractHistory(contractId: string): Observable<unknown[]> {
    return this.http.get<unknown>(`${this.base}/Contracts/${contractId}/history`).pipe(
      map(res => {
        const d = unwrapApiResponse<unknown>(res) ?? res;
        return Array.isArray(d) ? d : [];
      })
    );
  }

  updateContract(id: string, payload: unknown): Observable<unknown> {
    return this.http.put<unknown>(`${this.base}/Contracts/${id}`, payload).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  createContract(payload: unknown): Observable<unknown> {
    return this.http.post<unknown>(`${this.base}/Contracts`, payload).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  deleteContract(id: string): Observable<unknown> {
    return this.http.delete<unknown>(`${this.base}/Contracts/${id}`).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }
}
