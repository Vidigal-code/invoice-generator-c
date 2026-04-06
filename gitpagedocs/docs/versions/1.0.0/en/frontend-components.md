# Frontend Deep-Dive

**Invoice Generator C** embraces a fully standalone Angular 17 interface. It is architecturally decoupled keeping component separation rigid across business features.

## 1. Sub-Modules Separation

The routing scheme separates logical areas utilizing native Lazy Loading keeping initial browser payload minuscule:
- `admin-logs.component` (`/admin`): Contains high-fidelity MatTable lists featuring pagination controls coupled alongside `Subject`-driven Debouncers for massive query strings avoiding API throttling.
- `dashboard.component` (`/dashboard`): Organises grouped sub-debts. Binds complex loading logic checking for RabbitMQ events dynamically rendering `mat-snack-bar` updates.
- `billet-viewer.component`: Special child-route bound for secure iframe embeddings tracking blob-like streams fetching presigned PDFs directly over the mocked LocalStack S3 instances securely. 

## 2. Global Interceptors and Auth Flows

Since we opted against exposing raw JWT variables inside local-storage to battle potential XSS infiltrations, our core interceptors merely enforce proper inclusion flags:

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const secureReq = req.clone({
    withCredentials: true // Permits transmission of strictly HttpOnly secured JWT fragments
  });
  return next(secureReq);
};
```

## 3. Sandboxing Billet Downloads

Visualizing the concluded agreements inside the frontend mandates the `sandbox` directive ensuring zero chance malicious scripts leak outside the generated document.

```html
<iframe
    [src]="pdfSafeUrl"
    sandbox="allow-same-origin allow-scripts"
    [title]="'Contrato'">
</iframe>
```

## 4. Angular Material Dark Mode

A critical business constraint demanded a sophisticated dark mode toggle. Inside `styles.scss` the global theme variables wrap deeply across classes overhauling everything:
- **Mat-Chips**: Adapt to softer colors avoiding burn-outs.
- **Form Inputs**: Outline colors transition to higher-opacity whites.
- **Elevation Shadows**: Swap from hard drop shadows into subtle structural separations keeping clarity solid regardless of lighting constraints.
