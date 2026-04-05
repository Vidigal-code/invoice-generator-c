import { ComponentFixture, TestBed } from '@angular/core/testing';
import { signal } from '@angular/core';
import { of } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { DashboardComponent } from '../../app/features/dashboard/dashboard.component';
import { ApiService } from '../../app/core/services/api.service';
import { AppState } from '../../app/state/app.state';
import { NotificationDialogService } from '../../app/core/services/notification-dialog.service';

describe('DashboardComponent (Integration)', () => {
  let fixture: ComponentFixture<DashboardComponent>;
  let mockApi: jasmine.SpyObj<ApiService>;
  let mockAppState: Partial<AppState>;

  beforeEach(async () => {
    mockApi = jasmine.createSpyObj('ApiService', [
      'getContracts',
      'getAgreementHistory',
      'calculateDebt',
      'createAgreement'
    ]);
    mockApi.getContracts.and.returnValue(of([]));
    mockApi.getAgreementHistory.and.returnValue(of([]));

    mockAppState = {
      debtCalculationResult: signal<unknown>(null),
      currentUserRole: signal('User'),
      isAuthenticated: signal(true),
      activeContract: signal(null),
      currentUsername: signal('tester')
    };

    const dialogStub = {
      open: jasmine.createSpy('open').and.returnValue({ afterClosed: () => of(false) })
    };

    await TestBed.configureTestingModule({
      imports: [DashboardComponent, BrowserAnimationsModule],
      providers: [
        { provide: ApiService, useValue: mockApi },
        { provide: AppState, useValue: mockAppState },
        { provide: MatDialog, useValue: dialogStub },
        NotificationDialogService
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(DashboardComponent);
    fixture.detectChanges();
  });

  it('should create and load empty lists', () => {
    expect(fixture.componentInstance).toBeTruthy();
    expect(mockApi.getContracts).toHaveBeenCalled();
    expect(mockApi.getAgreementHistory).toHaveBeenCalled();
  });

  it('should show contracts section title', () => {
    const el = fixture.nativeElement as HTMLElement;
    expect(el.textContent).toContain('Contratos Ativos');
  });
});
