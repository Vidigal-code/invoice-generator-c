import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { unwrapApiResponse } from '../../http/api-response.mapper';
import { AuthLoginResult } from '../../models/auth-login-result.model';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly base = '/api';

  constructor(private readonly http: HttpClient) {}

  login(credentials: unknown): Observable<AuthLoginResult> {
    return this.http.post<unknown>(`${this.base}/Auth/login`, credentials).pipe(
      map((res): AuthLoginResult => {
        const data = unwrapApiResponse<AuthLoginResult>(res);
        if (data) {
          return data;
        }
        return typeof res === 'object' && res !== null ? (res as AuthLoginResult) : {};
      })
    );
  }

  register(credentials: unknown): Observable<unknown> {
    return this.http.post<unknown>(`${this.base}/Auth/register`, credentials).pipe(
      map(res => unwrapApiResponse(res) ?? res)
    );
  }
}
