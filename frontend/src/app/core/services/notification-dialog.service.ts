import { Injectable, inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { DialogPopupComponent } from '../../shared/components/ui/dialog-popup.component';

@Injectable({ providedIn: 'root' })
export class NotificationDialogService {
  private readonly dialog = inject(MatDialog);

  showSuccess(message: string, title = 'Sucesso'): void {
    this.dialog.open(DialogPopupComponent, {
      width: '380px',
      maxWidth: '92vw',
      data: { title, message, icon: 'check_circle', confirmLabel: 'OK' }
    });
  }

  showError(title: string, message: string): void {
    this.dialog.open(DialogPopupComponent, {
      width: '420px',
      maxWidth: '92vw',
      data: { title, message, icon: 'error_outline', confirmLabel: 'Fechar' }
    });
  }
}
