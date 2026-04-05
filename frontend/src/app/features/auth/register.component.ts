import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { NavbarComponent } from '../../shared/components/ui/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/ui/footer/footer.component';
import { strongPasswordValidator } from '../../core/validation/password-strength.validator';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule, NavbarComponent, FooterComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private apiService = inject(ApiService);
  private router = inject(Router);

  registerForm: FormGroup = this.fb.group({
    username: ['', [Validators.required, Validators.minLength(3)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, strongPasswordValidator()]]
  });

  isLoading = false;
  errorMsg = '';

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMsg = '';

      this.apiService.register(this.registerForm.value).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.isLoading = false;
          const body = err.error;
          if (body?.errors && typeof body.errors === 'object') {
            const msgs = Object.values(body.errors).flat() as string[];
            this.errorMsg = msgs[0] || body.message || 'Dados inválidos.';
          } else {
            this.errorMsg = body?.message || 'Falha ao registrar usuário na rede de Segurança.';
          }
        }
      });
    }
  }
}
