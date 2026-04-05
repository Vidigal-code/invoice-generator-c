import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { ApiService } from '../../core/services/api.service';
import { NotificationDialogService } from '../../core/services/notification-dialog.service';
import { DEFAULT_PORTFOLIO_OPTIONS, PORTFOLIO_API_PRIMARY } from '../../core/constants/portfolio.constants';
import { MAX_INSTALLMENTS } from '../../core/constants/negotiation-limits';
import { PortfolioLabelPipe } from '../../shared/pipes/portfolio-label.pipe';
import { DialogPopupComponent } from '../../shared/components/ui/dialog-popup.component';
import { ContractHistoryDialogComponent } from './dialogs/contract-history-dialog/contract-history-dialog.component';
import { ContractEditDialogComponent } from './dialogs/contract-edit-dialog/contract-edit-dialog.component';
import { ContractCreateDialogComponent } from './dialogs/contract-create-dialog/contract-create-dialog.component';
import { UserEditDialogComponent } from './dialogs/user-edit-dialog/user-edit-dialog.component';
import { ResetPasswordDialogComponent } from './dialogs/reset-password-dialog/reset-password-dialog.component';

@Component({
  selector: 'app-admin-logs',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatTabsModule,
    MatTableModule,
    MatChipsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatPaginatorModule,
    MatDialogModule,
    MatSlideToggleModule,
    PortfolioLabelPipe
  ],
  templateUrl: './admin-logs.component.html',
  styleUrl: './admin-logs.component.scss'
})
export class AdminLogsComponent implements OnInit {
  private readonly apiService = inject(ApiService);
  private readonly dialog = inject(MatDialog);
  private readonly fb = inject(FormBuilder);
  private readonly notify = inject(NotificationDialogService);

  sysLogs: string[] = [];
  sysLogFile = '';
  loadingSysLogs = false;

  auditLogs: any[] = [];
  auditTotal = 0;
  auditPage = 1;
  auditSize = 50;
  auditFilterEntity = '';
  auditFilterAction = '';
  loadingAudit = false;

  loginEvents: any[] = [];
  loginTotal = 0;
  loginPage = 1;
  loginSize = 50;
  loginColumns = ['action', 'username', 'ipAddress', 'userId'];
  loadingLogin = false;

  agreementActions: any[] = [];
  agreementTotal = 0;
  agreementPage = 1;
  agreementSize = 50;
  agreementColumns = ['action', 'entityId', 'newValues', 'ipAddress', 'userId'];
  loadingActions = false;

  contracts: any[] = [];
  contractsTotal = 0;
  contractsPage = 1;
  contractsSize = 20;
  contractSearch = '';
  contractStatusFilter = '';
  contractColumns = ['contractNumber', 'portfolio', 'debtorName', 'debtorDocument', 'owner', 'originalValue', 'currentBalance', 'status', 'history', 'actions'];
  readonly contractPortfolios = [...DEFAULT_PORTFOLIO_OPTIONS];
  loadingContracts = false;
  contractStatuses = [
    { label: 'Ativo', value: 'Active' },
    { label: 'Negociado', value: 'Negotiated' },
    { label: 'Inadimplente', value: 'Defaulted' },
    { label: 'Encerrado', value: 'Closed' },
    { label: 'Cancelado', value: 'Cancelled' }
  ];

  users: any[] = [];
  roles: any[] = [];
  userSearch = '';
  userColumns = ['username', 'email', 'roleName', 'isActive', 'actions'];
  loadingUsers = false;

  editContractForm!: FormGroup;
  createContractForm!: FormGroup;
  editUserForm!: FormGroup;

  ngOnInit(): void {
    this.loadSysLogs();
    this.loadAuditLogs();
    this.loadLoginEvents();
    this.loadAgreementActions();
    this.loadContracts();
    this.loadUsers();
    this.loadRoles();
  }

  loadSysLogs(): void {
    this.loadingSysLogs = true;
    this.apiService.getAdminLogs().subscribe({
      next: d => {
        this.sysLogs = d.logs?.length ? d.logs : [];
        this.sysLogFile = d.filename || '';
        this.loadingSysLogs = false;
      },
      error: () => {
        this.sysLogs = ['Erro ao carregar logs do sistema.'];
        this.loadingSysLogs = false;
      }
    });
  }

  loadAuditLogs(): void {
    this.loadingAudit = true;
    this.apiService
      .getAuditLogs(this.auditPage, this.auditSize, this.auditFilterEntity || undefined, this.auditFilterAction || undefined)
      .subscribe({
        next: d => {
          const payload = d as { items?: any[]; Items?: any[]; total?: number; Total?: number };
          this.auditLogs = payload?.items ?? payload?.Items ?? (Array.isArray(d) ? d : []);
          this.auditTotal = payload?.total ?? payload?.Total ?? 0;
          this.loadingAudit = false;
        },
        error: () => {
          this.loadingAudit = false;
        }
      });
  }

  onAuditPage(e: PageEvent): void {
    this.auditPage = e.pageIndex + 1;
    this.auditSize = e.pageSize;
    this.loadAuditLogs();
  }

  loadLoginEvents(): void {
    this.loadingLogin = true;
    this.apiService.getLoginEvents(this.loginPage, this.loginSize).subscribe({
      next: d => {
        const payload = d as { items?: any[]; Items?: any[]; total?: number; Total?: number };
        this.loginEvents = payload?.items ?? payload?.Items ?? (Array.isArray(d) ? d : []);
        this.loginTotal = payload?.total ?? payload?.Total ?? 0;
        this.loadingLogin = false;
      },
      error: () => {
        this.loadingLogin = false;
      }
    });
  }

  onLoginPage(e: PageEvent): void {
    this.loginPage = e.pageIndex + 1;
    this.loginSize = e.pageSize;
    this.loadLoginEvents();
  }

  loadAgreementActions(): void {
    this.loadingActions = true;
    this.apiService.getAgreementActions(this.agreementPage, this.agreementSize).subscribe({
      next: d => {
        const payload = d as { items?: any[]; Items?: any[]; total?: number; Total?: number };
        this.agreementActions = payload?.items ?? payload?.Items ?? (Array.isArray(d) ? d : []);
        this.agreementTotal = payload?.total ?? payload?.Total ?? 0;
        this.loadingActions = false;
      },
      error: () => {
        this.loadingActions = false;
      }
    });
  }

  onActionsPage(e: PageEvent): void {
    this.agreementPage = e.pageIndex + 1;
    this.agreementSize = e.pageSize;
    this.loadAgreementActions();
  }

  loadContracts(): void {
    this.loadingContracts = true;
    this.apiService
      .getAdminContracts(this.contractSearch || undefined, this.contractStatusFilter || undefined, this.contractsPage, this.contractsSize)
      .subscribe({
        next: d => {
          const payload = d as { items?: any[]; Items?: any[]; total?: number; Total?: number };
          this.contracts = payload?.items ?? payload?.Items ?? [];
          this.contractsTotal = payload?.total ?? payload?.Total ?? 0;
          this.loadingContracts = false;
        },
        error: () => {
          this.loadingContracts = false;
        }
      });
  }

  onContractPage(e: PageEvent): void {
    this.contractsPage = e.pageIndex + 1;
    this.contractsSize = e.pageSize;
    this.loadContracts();
  }

  contractOwnerLabel(ownerUserId: string | null | undefined): string {
    if (!ownerUserId) {
      return '—';
    }
    const u = this.users.find((x: { id?: string }) => x.id === ownerUserId);
    return u ? `${(u as { username?: string }).username}` : ownerUserId.slice(0, 8) + '…';
  }

  openEditContract(contract: any): void {
    this.editContractForm = this.fb.group({
      contractNumber: [contract.contractNumber, Validators.required],
      debtorName: [contract.debtorName, Validators.required],
      debtorDocument: [contract.debtorDocument, Validators.required],
      originalValue: [contract.originalValue, [Validators.required, Validators.min(0)]],
      currentBalance: [contract.currentBalance, [Validators.required, Validators.min(0)]],
      portfolio: [contract.portfolio ?? PORTFOLIO_API_PRIMARY, Validators.required],
      status: [contract.status, Validators.required],
      ownerUserId: [contract.ownerUserId ?? null]
    });

    const userOptions = this.users.map((u: { id: string; username: string; email: string }) => ({
      id: u.id,
      username: u.username,
      email: u.email
    }));

    const ref = this.dialog.open(ContractEditDialogComponent, {
      width: '560px',
      maxWidth: '96vw',
      data: {
        form: this.editContractForm,
        statuses: this.contractStatuses,
        portfolios: this.contractPortfolios,
        users: userOptions
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.apiService.updateContract(contract.id, this.editContractForm.value).subscribe({
        next: () => {
          this.loadContracts();
          this.loadAgreementActions();
          this.loadAuditLogs();
          this.notify.showSuccess('Contrato atualizado com sucesso!');
        },
        error: err => this.notify.showError('Erro ao atualizar contrato', err.error?.message || 'Erro desconhecido.')
      });
    });
  }

  openCreateContract(): void {
    const today = new Date();
    this.createContractForm = this.fb.group({
      contractNumber: ['', Validators.required],
      debtorName: ['', Validators.required],
      debtorDocument: ['', Validators.required],
      originalValue: [0, [Validators.required, Validators.min(0.01)]],
      currentBalance: [0, [Validators.required, Validators.min(0)]],
      portfolio: [PORTFOLIO_API_PRIMARY, Validators.required],
      status: ['Active', Validators.required],
      ownerUserId: [null as string | null],
      installmentsCount: [3, [Validators.required, Validators.min(1), Validators.max(MAX_INSTALLMENTS)]],
      firstDueDate: [today, Validators.required]
    });

    const userOptions = this.users.map((u: { id: string; username: string; email: string }) => ({
      id: u.id,
      username: u.username,
      email: u.email
    }));

    const ref = this.dialog.open(ContractCreateDialogComponent, {
      width: '560px',
      maxWidth: '96vw',
      data: {
        form: this.createContractForm,
        statuses: this.contractStatuses,
        portfolios: this.contractPortfolios,
        users: userOptions
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) {
        return;
      }
      const v = this.createContractForm.value;
      const d = v.firstDueDate as Date;
      const firstDueDateUtc = new Date(Date.UTC(d.getFullYear(), d.getMonth(), d.getDate())).toISOString();
      const payload = {
        contractNumber: v.contractNumber,
        debtorName: v.debtorName,
        debtorDocument: v.debtorDocument,
        originalValue: Number(v.originalValue),
        currentBalance: Number(v.currentBalance),
        portfolio: v.portfolio,
        status: v.status,
        ownerUserId: v.ownerUserId || null,
        installmentsCount: Number(v.installmentsCount),
        firstDueDateUtc
      };
      this.apiService.createContract(payload).subscribe({
        next: () => {
          this.loadContracts();
          this.loadAgreementActions();
          this.loadAuditLogs();
          this.notify.showSuccess('Contrato criado com sucesso.');
        },
        error: err => this.notify.showError('Erro ao criar contrato', err.error?.message || 'Erro desconhecido.')
      });
    });
  }

  openContractHistory(contract: any): void {
    this.apiService.getContractHistory(contract.id).subscribe({
      next: rows => {
        this.dialog.open(ContractHistoryDialogComponent, {
          width: '720px',
          maxWidth: '96vw',
          data: { rows: rows ?? [], contractNumber: contract.contractNumber }
        });
      },
      error: err =>
        this.notify.showError('Histórico do contrato', err.error?.message || 'Não foi possível carregar o histórico.')
    });
  }

  openDeleteContract(contract: any): void {
    const ref = this.dialog.open(DialogPopupComponent, {
      width: '420px',
      maxWidth: '92vw',
      data: {
        title: 'Confirmar Exclusão',
        message: `Deseja cancelar o contrato ${contract.contractNumber} – ${contract.debtorName}? Esta ação não pode ser desfeita.`,
        icon: 'warning',
        confirmLabel: 'Cancelar Contrato'
      }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.apiService.deleteContract(contract.id).subscribe({
        next: () => {
          this.loadContracts();
          this.loadAgreementActions();
          this.loadAuditLogs();
          this.notify.showSuccess('Contrato cancelado.');
        },
        error: err => this.notify.showError('Erro ao cancelar contrato', err.error?.message || 'Erro desconhecido.')
      });
    });
  }

  loadUsers(): void {
    this.loadingUsers = true;
    this.apiService.getAdminUsers(this.userSearch || undefined).subscribe({
      next: d => {
        this.users = d;
        this.loadingUsers = false;
      },
      error: () => {
        this.loadingUsers = false;
      }
    });
  }

  loadRoles(): void {
    this.apiService.getAdminRoles().subscribe({ next: r => (this.roles = r), error: () => {} });
  }

  openEditUser(user: any): void {
    this.editUserForm = this.fb.group({
      username: [user.username, Validators.required],
      email: [user.email, [Validators.required, Validators.email]],
      isActive: [user.isActive],
      roleId: [user.roleId, Validators.required]
    });

    const ref = this.dialog.open(UserEditDialogComponent, {
      width: '480px',
      maxWidth: '96vw',
      data: { form: this.editUserForm, roles: this.roles }
    });

    ref.afterClosed().subscribe(confirmed => {
      if (!confirmed) return;
      this.apiService.updateAdminUser(user.id, this.editUserForm.value).subscribe({
        next: () => {
          this.loadUsers();
          this.notify.showSuccess('Usuário atualizado!');
        },
        error: err => this.notify.showError('Erro ao atualizar', err.error?.message || 'Erro.')
      });
    });
  }

  openResetPassword(user: any): void {
    const ref = this.dialog.open(ResetPasswordDialogComponent, {
      width: '400px',
      maxWidth: '96vw',
      data: { username: user.username }
    });

    ref.afterClosed().subscribe((newPassword: string | null) => {
      if (!newPassword) return;
      this.apiService.resetUserPassword(user.id, newPassword).subscribe({
        next: () => this.notify.showSuccess('Senha redefinida com sucesso!'),
        error: err => this.notify.showError('Erro ao redefinir senha', err.error?.message || 'Erro.')
      });
    });
  }
}
