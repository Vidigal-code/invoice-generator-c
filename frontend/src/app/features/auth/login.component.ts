import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { Router } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { AppState } from '../../state/app.state';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { NavbarComponent } from '../../shared/components/ui/navbar/navbar.component';
import { FooterComponent } from '../../shared/components/ui/footer/footer.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, FormsModule, ReactiveFormsModule, RouterModule,
    NavbarComponent, FooterComponent,
    MatCardModule, MatInputModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage = '';

  private apiService = inject(ApiService);
  private appState = inject(AppState);
  private router = inject(Router);

  constructor(private fb: FormBuilder) {
    this.loginForm = this.fb.group({
      email: ['', Validators.required],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      this.apiService.login(this.loginForm.value).subscribe({
        next: (res) => {
          this.isLoading = false;
          this.appState.setLoginState(res.role || 'User', res.username || res.email || '');
          this.router.navigate(['/dashboard']);
        },
        error: (_err) => {
          this.isLoading = false;
          this.errorMessage = 'Credenciais inválidas ou erro no servidor.';
        }
      });
    }
  }
}
