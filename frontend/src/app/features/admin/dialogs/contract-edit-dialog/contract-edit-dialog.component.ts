import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormGroup } from '@angular/forms';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import type { PortfolioOption } from '../../../../core/constants/portfolio.constants';

export interface SelectOption {
  label: string;
  value: string;
}

export interface AdminUserOption {
  id: string;
  username: string;
  email: string;
}

export interface ContractEditDialogData {
  form: FormGroup;
  statuses: SelectOption[];
  portfolios: readonly PortfolioOption[];
  users: AdminUserOption[];
}

@Component({
  selector: 'app-contract-edit-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule
  ],
  templateUrl: './contract-edit-dialog.component.html',
  styleUrl: './contract-edit-dialog.component.scss'
})
export class ContractEditDialogComponent {
  constructor(
    public readonly ref: MatDialogRef<ContractEditDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public readonly data: ContractEditDialogData
  ) {}
}
