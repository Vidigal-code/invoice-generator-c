import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatDividerModule } from '@angular/material/divider';
import { ApiService } from '../../core/services/api.service';
import { NotificationDialogService } from '../../core/services/notification-dialog.service';
import { contractStatusLabel, agreementHistoryDisplayLabel } from '../../core/constants/contract-status.constants';
import { AppState } from '../../state/app.state';
import { DialogPopupComponent } from '../../shared/components/ui/dialog-popup.component';
import { PortfolioLabelPipe } from '../../shared/pipes/portfolio-label.pipe';
import { BilletViewerComponent } from './billet-viewer/billet-viewer.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, FormsModule,
    MatCardModule, MatTableModule, MatButtonModule, MatDialogModule,
    MatProgressSpinnerModule, MatIconModule, MatChipsModule,
    MatTooltipModule, MatSelectModule, MatFormFieldModule, MatDividerModule,
    BilletViewerComponent,
    PortfolioLabelPipe
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent implements OnInit {
  private readonly apiService = inject(ApiService);
  private readonly dialog = inject(MatDialog);
  private readonly notify = inject(NotificationDialogService);
  public readonly appState = inject(AppState);

  readonly contractStatusLabel = contractStatusLabel;
  readonly agreementHistoryLabel = agreementHistoryDisplayLabel;

  contractColumns: string[] = ['contractNumber', 'portfolio', 'debtorName', 'status', 'action'];
  historyColumns: string[] = ['contract', 'contractStatus', 'value', 'installments', 'date', 'status', 'action'];
  installmentColumns: string[] = ['number', 'due', 'original', 'current', 'overdue'];

  contractsList: any[] = [];
  historyList: any[] = [];
  selectedBilletId: string | null = null;

  loadingContracts = false;
  loadingHistory = false;
  loadingCalc = false;
  loadingFormalize = false;

  selectedInstallments = 3;
  installmentOptions = [1, 2, 3, 6, 9, 12, 18, 24];

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.loadContracts();
    this.loadHistory();
  }

  loadContracts() {
    this.loadingContracts = true;
    this.apiService.getContracts().subscribe({
      next: (data) => { this.contractsList = data; this.loadingContracts = false; },
      error: () => {
        this.loadingContracts = false;
        this.notify.showError(
          'Erro ao Carregar Contratos',
          'Não foi possível buscar os contratos do servidor. Verifique a conexão com o backend.'
        );
      }
    });
  }

  loadHistory() {
    this.loadingHistory = true;
    this.apiService.getAgreementHistory().subscribe({
      next: (data) => { this.historyList = data; this.loadingHistory = false; },
      error: () => { this.loadingHistory = false; }
    });
  }

  simulate(contractId: string) {
    this.loadingCalc = true;
    this.apiService.calculateDebt(contractId).subscribe({
      next: (res) => { this.appState.debtCalculationResult.set(res); this.loadingCalc = false; },
      error: (err) => {
        this.loadingCalc = false;
        this.notify.showError(
          'Erro no Cálculo',
          err.error?.errors?.message || 'Não foi possível calcular a dívida para este contrato.'
        );
      }
    });
  }

  formalizeAgreement(contractId: string | undefined) {
    if (!contractId) return;
    this.loadingFormalize = true;

    const payload = {
      contractId,
      negotiatedValue: this.appState.debtCalculationResult()?.currentTotalDebt,
      installmentsCount: this.selectedInstallments
    };

    this.apiService.createAgreement(payload).subscribe({
      next: (res) => {
        this.loadingFormalize = false;
        this.appState.debtCalculationResult.set(null);
        this.dialog.open(DialogPopupComponent, {
          width: '420px', maxWidth: '92vw',
          data: {
            title: 'Acordo Formalizado!',
            message: `Acordo criado com sucesso em ${this.selectedInstallments} parcela(s). Os boletos estão disponíveis no histórico.`,
            icon: 'check_circle',
            confirmLabel: 'Fechar'
          }
        });
        this.loadHistory();
        this.loadContracts();
      },
      error: (err) => {
        this.loadingFormalize = false;

        const apiMsg: string | undefined = err?.error?.errors?.message || err?.error?.message;
        const status = err?.status;

        let msg: string;

        if (apiMsg) {
          msg = apiMsg;
        } else if (status === 409) {
          msg = 'Este contrato já possui um acordo ativo. Acesse o histórico para mais detalhes.';
        } else if (status === 401 || status === 403) {
          msg = 'Sessão expirada. Por favor, faça login novamente.';
        } else {
          msg = 'Não foi possível formalizar o acordo. Tente novamente em instantes.';
        }

        this.notify.showError('Atenção', msg);
      }
    });
  }


  downloadPdf(billetId: string) {
    this.apiService.downloadBilletPdf(billetId);
  }

  toggleBilletViewer(billetId: string): void {
    this.selectedBilletId = this.selectedBilletId === billetId ? null : billetId;
  }

}
