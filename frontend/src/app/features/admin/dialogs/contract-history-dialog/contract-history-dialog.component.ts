import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';

export interface ContractHistoryDialogData {
  rows: unknown[];
  contractNumber: string;
}

@Component({
  selector: 'app-contract-history-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatTableModule],
  templateUrl: './contract-history-dialog.component.html',
  styleUrl: './contract-history-dialog.component.scss'
})
export class ContractHistoryDialogComponent {
  readonly historyColumns: string[] = ['occurredAt', 'changeType', 'payloadJson'];

  constructor(@Inject(MAT_DIALOG_DATA) public readonly data: ContractHistoryDialogData) {}
}
