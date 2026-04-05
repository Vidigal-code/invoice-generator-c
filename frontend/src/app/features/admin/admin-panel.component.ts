import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService } from '../../core/services/api.service';

@Component({
    selector: 'app-admin-panel',
    standalone: true,
    imports: [CommonModule],
    template: `
    <div class="admin-container">
      <div class="header-banner">
        <h2>Corporate Observability Panel</h2>
        <p>Acompanhamento de rotatividade Serilog e Traços de Auditoria (Admin Only)</p>
      </div>

      <div class="alert alert-error" *ngIf="errorMsg">
        {{ errorMsg }}
      </div>

      <div class="logs-card" *ngIf="!isLoading && logs.length > 0">
        <div class="logs-header">
          <strong>Arquivo ativo:</strong> {{ activeFile }} | <strong>Última atualização:</strong> {{ lastFetch | date:'HH:mm:ss' }}
          <button class="btn btn-sm" (click)="fetchLogs()">Atualizar Agora</button>
        </div>
        <div class="terminal-window">
          <div *ngFor="let log of logs" class="log-line">
            {{ log }}
          </div>
        </div>
      </div>

      <div *ngIf="isLoading" class="loading-state">
        <span class="spinner"></span> Carregando auditoria encriptada...
      </div>
    </div>
  `,
    styles: [`
    .admin-container { padding: 2rem; max-width: 1200px; margin: 0 auto; font-family: 'Inter', sans-serif; }
    .header-banner { background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%); color: white; padding: 2rem; border-radius: 12px; margin-bottom: 2rem; box-shadow: 0 10px 25px rgba(0,0,0,0.1); }
    .header-banner h2 { margin: 0 0 0.5rem 0; font-size: 1.8rem; }
    .header-banner p { margin: 0; color: #94a3b8; font-size: 0.95rem; }
    
    .logs-card { background: #fff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); overflow: hidden; border: 1px solid #e2e8f0; }
    .logs-header { background: #f8fafc; padding: 1rem 1.5rem; border-bottom: 1px solid #e2e8f0; display: flex; justify-content: space-between; align-items: center; font-size: 0.9rem; color: #475569; }
    .btn-sm { background: #3b82f6; color: white; border: none; padding: 0.5rem 1rem; border-radius: 6px; cursor: pointer; font-size: 0.85rem; font-weight: 500; transition: background 0.2s; }
    .btn-sm:hover { background: #2563eb; }
    
    .terminal-window { background: #0f172a; color: #33ff00; padding: 1.5rem; height: 60vh; overflow-y: auto; font-family: 'Fira Code', 'Courier New', monospace; font-size: 0.85rem; line-height: 1.5; }
    .log-line { margin-bottom: 0.2rem; word-break: break-all; border-bottom: 1px solid rgba(255,255,255,0.05); padding-bottom: 0.2rem; }
    .log-line:hover { background-color: rgba(255,255,255,0.05); }

    .alert-error { background-color: #fef2f2; color: #ef4444; border: 1px solid #fca5a5; padding: 1rem; border-radius: 8px; margin-bottom: 1rem; }
    
    .loading-state { text-align: center; padding: 4rem; color: #64748b; font-weight: 500; }
    .spinner { display: inline-block; width: 1.5rem; height: 1.5rem; border: 3px solid #cbd5e1; border-radius: 50%; border-top-color: #3b82f6; animation: spin 1s linear infinite; vertical-align: middle; margin-right: 0.5rem; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `]
})
export class AdminPanelComponent implements OnInit {
    private apiService = inject(ApiService);

    logs: string[] = [];
    activeFile: string = '';
    lastFetch: Date = new Date();
    isLoading = false;
    errorMsg = '';

    ngOnInit(): void {
        this.fetchLogs();
    }

    fetchLogs(): void {
        this.isLoading = true;
        this.errorMsg = '';
        this.apiService.getAdminLogs().subscribe({
            next: (response) => {
                this.logs = response.logs || [];
                this.activeFile = response.filename;
                this.lastFetch = new Date();
                this.isLoading = false;
            },
            error: (err) => {
                this.errorMsg = err.status === 403 ? 'Acesso negado: Perfil sem permissão de Administrador.' : 'Não foi possível carregar os logs do sistema.';
                this.isLoading = false;
            }
        });
    }
}
