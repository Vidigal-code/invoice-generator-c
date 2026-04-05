import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

export interface DialogPopupData {
  title: string;
  message: string;
  icon?: string;
  confirmLabel?: string;
}

@Component({
  selector: 'app-dialog-popup',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './dialog-popup.component.html',
  styleUrl: './dialog-popup.component.scss'
})
export class DialogPopupComponent {
  constructor(
    public dialogRef: MatDialogRef<DialogPopupComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogPopupData
  ) { }

  close(): void {
    this.dialogRef.close();
  }
}
