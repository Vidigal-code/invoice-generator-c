import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import type { PortfolioOption } from '../../../../core/constants/portfolio.constants';
import { MAX_INSTALLMENTS } from '../../../../core/constants/negotiation-limits';
import type { AdminUserOption, SelectOption } from '../contract-edit-dialog/contract-edit-dialog.component';

export interface ContractCreateDialogData {
  form: FormGroup;
  statuses: SelectOption[];
  portfolios: readonly PortfolioOption[];
  users: AdminUserOption[];
}

@Component({
  selector: 'app-contract-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatDatepickerModule
  ],
  templateUrl: './contract-create-dialog.component.html',
  styleUrl: './contract-create-dialog.component.scss'
})
export class ContractCreateDialogComponent {
  readonly maxInstallments = MAX_INSTALLMENTS;

  constructor(
    public readonly ref: MatDialogRef<ContractCreateDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public readonly data: ContractCreateDialogData
  ) {}
}
