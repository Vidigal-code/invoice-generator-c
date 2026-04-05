import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { unwrapApiResponse } from '../../http/api-response.mapper';

@Injectable({ providedIn: 'root' })
export class DebtAgreementsBilletsApiService {
  private readonly base = '/api';

  constructor(private readonly http: HttpClient) {}

  calculateDebt(contractId: string): Observable<unknown> {
    return this.http.get<unknown>(`${this.base}/Debt/${contractId}/calculate`).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  createAgreement(payload: unknown): Observable<unknown> {
    return this.http.post<unknown>(`${this.base}/Agreements`, payload).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }

  getAgreementHistory(): Observable<unknown[]> {
    return this.http.get<unknown>(`${this.base}/Agreements/history`).pipe(
      map(res => {
        const d = unwrapApiResponse<unknown>(res);
        return Array.isArray(d) ? d : [];
      })
    );
  }

  downloadBilletPdf(billetId: string): void {
    window.open(`${this.base}/Billets/${billetId}/pdf`, '_blank');
  }

  getBilletBlob(billetId: string): Observable<Blob> {
    return this.http.get(`${this.base}/Billets/${billetId}/pdf`, { responseType: 'blob' });
  }
}
