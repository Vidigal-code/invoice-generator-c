import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';

export interface ResetPasswordDialogData {
  username: string;
}

@Component({
  selector: 'app-reset-password-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatButtonModule, MatFormFieldModule, MatInputModule],
  templateUrl: './reset-password-dialog.component.html',
  styleUrl: './reset-password-dialog.component.scss'
})
export class ResetPasswordDialogComponent {
  newPassword = '';

  constructor(
    public readonly ref: MatDialogRef<ResetPasswordDialogComponent, string | null>,
    @Inject(MAT_DIALOG_DATA) public readonly data: ResetPasswordDialogData
  ) {}
}
