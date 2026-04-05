import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { of } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { AdminLogsComponent } from '../../app/features/admin/admin-logs.component';
import { ApiService } from '../../app/core/services/api.service';
import { NotificationDialogService } from '../../app/core/services/notification-dialog.service';

describe('AdminLogsComponent (smoke)', () => {
  let fixture: ComponentFixture<AdminLogsComponent>;

  beforeEach(async () => {
    const apiStub: Partial<ApiService> = {
      getAdminLogs: () => of({ logs: [], filename: '' }),
      getAuditLogs: () => of({ items: [], total: 0 }),
      getLoginEvents: () => of({ items: [], total: 0 }),
      getAgreementActions: () => of({ items: [], total: 0 }),
      getAdminContracts: () => of({ items: [], total: 0 }),
      getAdminUsers: () => of([]),
      getAdminRoles: () => of([])
    };

    const dialogStub = {
      open: jasmine.createSpy('open').and.returnValue({ afterClosed: () => of(false) })
    };

    await TestBed.configureTestingModule({
      imports: [AdminLogsComponent],
      providers: [
        { provide: ApiService, useValue: apiStub },
        { provide: MatDialog, useValue: dialogStub },
        NotificationDialogService
      ],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminLogsComponent);
  });

  it('should create', () => {
    fixture.detectChanges();
    expect(fixture.componentInstance).toBeTruthy();
  });
});
