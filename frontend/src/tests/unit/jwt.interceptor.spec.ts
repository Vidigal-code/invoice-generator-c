import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { jwtInterceptor } from '../../app/core/interceptors/jwt.interceptor';

describe('JwtInterceptor (Unit)', () => {
    let http: HttpClient;
    let httpMock: HttpTestingController;

    beforeEach(() => {

        spyOn(localStorage, 'getItem').and.callFake((key: string) => {
            return key === 'token' ? 'mocked-jwt-token' : null;
        });

        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [
                provideHttpClient(withInterceptors([jwtInterceptor]))
            ]
        });

        http = TestBed.inject(HttpClient);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should add Authorization header if token exists in localStorage', () => {
        http.get('/api/test-endpoint').subscribe();

        const req = httpMock.expectOne('/api/test-endpoint');
        expect(req.request.headers.has('Authorization')).toBeTrue();
        expect(req.request.headers.get('Authorization')).toBe('Bearer mocked-jwt-token');
        req.flush({});
    });
});
