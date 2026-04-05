import { HttpInterceptorFn } from '@angular/common/http';

export const credentialsInterceptor: HttpInterceptorFn = (req, next) => {

  // Gera ID universal para manter rastreio no Backend Logger
    const correlationId = crypto.randomUUID();

    // Always attach cross-origin cookies to requests and Trace UUIDs
    req = req.clone({
        withCredentials: true,
        setHeaders: {
            'X-Correlation-ID': correlationId
        }
    });

    return next(req);
};
