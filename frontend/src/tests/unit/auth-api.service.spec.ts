import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthApiService } from '../../app/core/services/api/auth-api.service';

describe('AuthApiService', () => {
  let service: AuthApiService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthApiService]
    });
    service = TestBed.inject(AuthApiService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should POST login and unwrap data', () => {
    const creds = { email: 'a@b.c', password: 'x' };
    service.login(creds).subscribe(res => {
      expect(res).toEqual({ token: 't' });
    });
    const req = httpMock.expectOne('/api/Auth/login');
    expect(req.request.method).toBe('POST');
    req.flush({ success: true, data: { token: 't' } });
  });
});
