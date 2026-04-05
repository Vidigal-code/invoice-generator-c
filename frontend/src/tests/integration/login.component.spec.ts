import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from '../../app/features/auth/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { ApiService } from '../../app/core/services/api.service';
import { AppState } from '../../app/state/app.state';
import { Router } from '@angular/router';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { MatCardModule } from '@angular/material/card';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

describe('LoginComponent (Integration)', () => {
    let component: LoginComponent;
    let fixture: ComponentFixture<LoginComponent>;
    let mockApiService: jasmine.SpyObj<ApiService>;
    let mockAppState: jasmine.SpyObj<AppState>;
    let mockRouter: jasmine.SpyObj<Router>;

    beforeEach(async () => {
        mockApiService = jasmine.createSpyObj('ApiService', ['login']);
        mockAppState = jasmine.createSpyObj('AppState', ['setLoginState']);
        mockRouter = jasmine.createSpyObj('Router', ['navigate']);

        await TestBed.configureTestingModule({
            imports: [
                LoginComponent,
                ReactiveFormsModule,
                MatCardModule,
                MatInputModule,
                MatButtonModule,
                BrowserAnimationsModule
            ],
            providers: [
                { provide: ApiService, useValue: mockApiService },
                { provide: AppState, useValue: mockAppState },
                { provide: Router, useValue: mockRouter }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(LoginComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create the login component', () => {
        expect(component).toBeTruthy();
    });

    it('should render login form with email and password fields', () => {
        const compiled = fixture.nativeElement as HTMLElement;
        expect(compiled.querySelector('input[formControlName="email"]')).toBeTruthy();
        expect(compiled.querySelector('input[formControlName="password"]')).toBeTruthy();
        expect(compiled.querySelector('button[type="submit"]')).toBeTruthy();
    });

    it('should show error message on failed login', () => {

        component.loginForm.patchValue({ email: 'test@test.com', password: 'wrongpassword' });

        mockApiService.login.and.returnValue(throwError(() => new Error('Unauthorized')));

        component.onSubmit();
        fixture.detectChanges();

        expect(mockApiService.login).toHaveBeenCalled();
        expect(component.errorMessage).toBe('Credenciais inválidas ou erro no servidor.');

        const compiled = fixture.nativeElement as HTMLElement;
        const errorMsgDiv = compiled.querySelector('.error-msg');
        expect(errorMsgDiv?.textContent).toContain('Credenciais inválidas');
    });

    it('should navigate to dashboard and set app state on success login', () => {
        component.loginForm.patchValue({ email: 'admin@test.com', password: 'password123' });
        mockApiService.login.and.returnValue(of({ token: 'jwt123', role: 'Admin', email: 'admin@test.com' }));

        component.onSubmit();

        expect(mockApiService.login).toHaveBeenCalled();
        expect(mockAppState.setLoginState).toHaveBeenCalledWith('Admin', 'admin@test.com');
        expect(mockRouter.navigate).toHaveBeenCalledWith(['/dashboard']);
    });
});
