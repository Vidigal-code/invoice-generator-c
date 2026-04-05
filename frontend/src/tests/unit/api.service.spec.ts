import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ApiService } from '../../app/core/services/api.service';

describe('ApiService (facade)', () => {
  let service: ApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule]
    });
    service = TestBed.inject(ApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should POST login via AuthApiService delegation', () => {
    const body = { email: 'admin@test.local', password: 'Secret1!' };
    service.login(body).subscribe(res => {
      expect(res).toEqual({ token: 'jwt' });
    });
    const req = httpMock.expectOne('/api/Auth/login');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(body);
    req.flush({ success: true, data: { token: 'jwt' } });
  });

  it('should GET debt calculation', () => {
    const id = '550e8400-e29b-41d4-a716-446655440000';
    service.calculateDebt(id).subscribe(res => {
      expect(res).toEqual({ contractId: id });
    });
    const req = httpMock.expectOne(r => r.url.includes(`/api/Debt/${id}/calculate`));
    expect(req.request.method).toBe('GET');
    req.flush({ success: true, data: { contractId: id } });
  });
});
