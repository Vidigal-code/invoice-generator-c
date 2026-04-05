import { Component, Input, OnChanges, OnDestroy, SimpleChanges, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { ApiService } from '../../../core/services/api.service';

@Component({
  selector: 'app-billet-viewer',
  standalone: true,
  imports: [CommonModule, MatProgressSpinnerModule, MatButtonModule, MatIconModule, MatTooltipModule],
  templateUrl: './billet-viewer.component.html',
  styleUrl: './billet-viewer.component.scss'
})
export class BilletViewerComponent implements OnChanges, OnDestroy {
  @Input() billetId: string | null = null;

  private apiService = inject(ApiService);
  private sanitizer = inject(DomSanitizer);

  iframeSrc: SafeResourceUrl | null = null;

  iframeSandbox: string | null = 'allow-same-origin allow-scripts allow-modals';
  loading = false;
  errorMessage: string | null = null;

  private rawObjectUrl: string | null = null;
  private displayBlob: Blob | null = null;

  displayIsPdf = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['billetId']) {
      if (this.billetId) {
        this.load();
      } else {
        this.clear();
      }
    }
  }

  ngOnDestroy(): void {
    this.revokeObjectUrl();
  }

  load(): void {
    if (!this.billetId) return;
    this.loading = true;
    this.errorMessage = null;
    this.revokeObjectUrl();
    this.iframeSrc = null;
    this.displayBlob = null;

    this.apiService.getBilletBlob(this.billetId).subscribe({
      next: async (blob: Blob) => {
        try {
          const isPdf = await this.blobLooksPdf(blob);
          const displayBlob = isPdf
            ? new Blob([await blob.arrayBuffer()], { type: 'application/pdf' })
            : blob.type.startsWith('text/html')
              ? blob
              : new Blob([await blob.text()], { type: 'text/html;charset=utf-8' });

          this.displayBlob = displayBlob;
          this.displayIsPdf = isPdf;
          this.iframeSandbox = isPdf ? null : 'allow-same-origin allow-scripts allow-modals';

          const url = URL.createObjectURL(displayBlob);
          this.rawObjectUrl = url;
          this.iframeSrc = this.sanitizer.bypassSecurityTrustResourceUrl(url);
        } catch {
          this.errorMessage = 'Não foi possível processar o boleto.';
        } finally {
          this.loading = false;
        }
      },
      error: () => {
        this.errorMessage = 'Não foi possível carregar o boleto. Tente novamente.';
        this.loading = false;
      }
    });
  }

  private blobLooksPdf(blob: Blob): Promise<boolean> {
    if (blob.type === 'application/pdf') {
      return Promise.resolve(true);
    }
    return blob.slice(0, 5).arrayBuffer().then(ab => {
      const u = new Uint8Array(ab);
      return u.length >= 4 && u[0] === 0x25 && u[1] === 0x50 && u[2] === 0x44 && u[3] === 0x46;
    });
  }

  clear(): void {
    this.revokeObjectUrl();
    this.iframeSrc = null;
    this.errorMessage = null;
    this.loading = false;
    this.displayBlob = null;
    this.displayIsPdf = false;
    this.iframeSandbox = 'allow-same-origin allow-scripts allow-modals';
  }

  private revokeObjectUrl(): void {
    if (this.rawObjectUrl) {
      URL.revokeObjectURL(this.rawObjectUrl);
      this.rawObjectUrl = null;
    }
  }

  printSlip(): void {
    const iframe = document.querySelector('.billet-iframe') as HTMLIFrameElement;
    if (iframe?.contentWindow) {
      iframe.contentWindow.focus();
      iframe.contentWindow.print();
    }
  }

  openInNewTab(): void {
    if (this.rawObjectUrl) {
      window.open(this.rawObjectUrl, '_blank', 'noopener,noreferrer');
      return;
    }
    this.ensureBlobThen(url => window.open(url, '_blank', 'noopener,noreferrer'));
  }

  downloadSlip(): void {
    if (!this.billetId) return;
    this.ensureBlobThen(url => {
      const ext = this.displayIsPdf ? 'pdf' : 'html';
      const a = document.createElement('a');
      a.href = url;
      a.download = `Boleto_${this.billetId}.${ext}`;
      a.rel = 'noopener';
      a.click();
      if (url.startsWith('blob:') && url !== this.rawObjectUrl) {
        URL.revokeObjectURL(url);
      }
    });
  }

  private ensureBlobThen(useUrl: (url: string) => void): void {
    if (this.displayBlob) {
      const url = URL.createObjectURL(this.displayBlob);
      useUrl(url);
      if (url !== this.rawObjectUrl) {
        setTimeout(() => URL.revokeObjectURL(url), 60_000);
      }
      return;
    }
    if (!this.billetId) return;
    this.apiService.getBilletBlob(this.billetId).subscribe({
      next: async blob => {
        const isPdf = await this.blobLooksPdf(blob);
        const b = isPdf
          ? new Blob([await blob.arrayBuffer()], { type: 'application/pdf' })
          : blob.type.startsWith('text/html')
            ? blob
            : new Blob([await blob.text()], { type: 'text/html;charset=utf-8' });
        const url = URL.createObjectURL(b);
        useUrl(url);
        setTimeout(() => URL.revokeObjectURL(url), 60_000);
      },
      error: () => {}
    });
  }
}
