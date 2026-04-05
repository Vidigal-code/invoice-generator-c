import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthLoginResult } from '../models/auth-login-result.model';
import { AuthApiService } from './api/auth-api.service';
import { ContractsApiService } from './api/contracts-api.service';
import { DebtAgreementsBilletsApiService } from './api/debt-agreements-billets-api.service';
import { AdminPanelApiService } from './api/admin-panel-api.service';

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(
    private readonly auth: AuthApiService,
    private readonly contracts: ContractsApiService,
    private readonly debtAgreementsBillets: DebtAgreementsBilletsApiService,
    private readonly adminPanel: AdminPanelApiService
  ) {}

  login(credentials: unknown): Observable<AuthLoginResult> {
    return this.auth.login(credentials);
  }

  register(credentials: unknown): Observable<unknown> {
    return this.auth.register(credentials);
  }

  getContracts(activeOnly = true): Observable<any[]> {
    return this.contracts.getContracts(activeOnly) as Observable<any[]>;
  }

  calculateDebt(contractId: string): Observable<unknown> {
    return this.debtAgreementsBillets.calculateDebt(contractId);
  }

  createAgreement(payload: unknown): Observable<unknown> {
    return this.debtAgreementsBillets.createAgreement(payload);
  }

  getAgreementHistory(): Observable<any[]> {
    return this.debtAgreementsBillets.getAgreementHistory() as Observable<any[]>;
  }

  downloadBilletPdf(billetId: string): void {
    this.debtAgreementsBillets.downloadBilletPdf(billetId);
  }

  getBilletBlob(billetId: string): Observable<Blob> {
    return this.debtAgreementsBillets.getBilletBlob(billetId);
  }

  getAdminLogs(): Observable<{ logs: string[]; filename: string }> {
    return this.adminPanel.getAdminLogs();
  }

  getAuditLogs(page = 1, size = 50, entity?: string, action?: string): Observable<unknown> {
    return this.adminPanel.getAuditLogs(page, size, entity, action);
  }

  getLoginEvents(page = 1, size = 50): Observable<unknown> {
    return this.adminPanel.getLoginEvents(page, size);
  }

  getAgreementActions(page = 1, size = 50): Observable<unknown> {
    return this.adminPanel.getAgreementActions(page, size);
  }

  getAdminUsers(search?: string): Observable<any[]> {
    return this.adminPanel.getAdminUsers(search) as Observable<any[]>;
  }

  updateAdminUser(id: string, payload: unknown): Observable<unknown> {
    return this.adminPanel.updateAdminUser(id, payload);
  }

  resetUserPassword(id: string, newPassword: string): Observable<unknown> {
    return this.adminPanel.resetUserPassword(id, newPassword);
  }

  getAdminRoles(): Observable<any[]> {
    return this.adminPanel.getAdminRoles() as Observable<any[]>;
  }

  getAdminContracts(search?: string, status?: string, page = 1, size = 20): Observable<any> {
    return this.contracts.getAdminContracts(search, status, page, size) as Observable<any>;
  }

  getContractById(id: string): Observable<unknown> {
    return this.contracts.getContractById(id);
  }

  getContractHistory(contractId: string): Observable<any[]> {
    return this.contracts.getContractHistory(contractId) as Observable<any[]>;
  }

  updateContract(id: string, payload: unknown): Observable<unknown> {
    return this.contracts.updateContract(id, payload);
  }

  createContract(payload: unknown): Observable<unknown> {
    return this.contracts.createContract(payload);
  }

  deleteContract(id: string): Observable<unknown> {
    return this.contracts.deleteContract(id);
  }
}
