# Security and Audit Paradigms

Security sits at the epicenter of **Invoice Generator C**. Because of financial transaction requirements, the system aggressively implements Role-Based Access Control (RBAC), anti-forgery, and advanced auditing.

## 1. Role-Based Access Control (RBAC)

Authentication yields an explicit, stateless JWT assigned squarely mapping individual Claims (e.g., `Admin`, `Support`, `Guest`).
- **RouteProtectionMiddleware**: An impenetrable gatekeeper intercepting `/api/x` pipelines unconditionally. It matches current extracted claims against route permissions predefined on the platform layer, automatically dumping intruders to `HTTP 403 Forbidden` limits.
- **Tenant Isolation**: Beyond horizontal endpoints, vertical protection ensures User `Alpha` possessing "Admin" roles on Tenant A absolutely fails attempting enumeration over Tenant B resources holding identical valid roles globally.

## 2. IP Masking via AES-256

Data Privacy regulations demand IP retention alongside hiding.
The raw IP strings logged actively onto `AuditService` are dynamically truncated and **encrypted via AES-256** right prior database entry.

`192.168.10.150` -> `192.168.*.* [U2FsdGVkX18a6X...]`

The key sits highly coupled inside environment boundaries avoiding exposure. This provides compliance standards retaining tracking exactings if legal procedures trigger decrypt-orders explicitly.

## 3. Defense Against XSS and CSRF 

Traditional Token architectures leak easily exposing users against cross-site exploitation.
- **XSS Nullified**: The generated JWT string traverses backend outputs only encased as an attached *Cookie Component* flagged intrinsically possessing the `HttpOnly` marker. No frontend component (not even our Angular framework natively) retains Javascript reading capacity to clone the token.
- **SameSite and Secure Headers**: Cookies are transmitted exclusively matching `SameSite=Strict` bounded naturally inside `Secure` contexts over `HTTPS` proxies negating token injection loopholes across domains natively.
