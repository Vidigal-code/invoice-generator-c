# Architecture Overview

**Invoice Generator C** was built using a rigorous Clean Architecture pattern on the Backend and Modular componentization on the Frontend. It is designed to be Cloud-Native and supports robust asynchronous processing.

## Backend Architecture

The backend (.NET 8) is organized in layers aiming at decoupling and maintainability:
- **API**: REST endpoints exposition and `Program.cs` setup. Uses security middlewares (e.g., `RouteProtectionMiddleware`, `AuditLogMiddleware`).
- **Application**: Logic orchestrated by CQRS pattern (using `MediatR`). Contains detailed Handlers, Commands, and Queries.
- **Domain**: Essential business rules, Entities, and Repository Interfaces.
- **Infrastructure**: Concrete implementations of Repositories (`EF Core`), messaging (`MassTransit` with RabbitMQ), caching, and S3 storage (via LocalStack).

### Key Patterns Used
- **Distributed Locking:** The `RedisDistributedLock` ensures no race conditions occur when formalizing agreements.
- **Strategy Pattern:** Used in `InvoiceGeneratorCDebtCalculationStrategy` for flexibility in how billets or portfolios are calculated.
- **Event-Driven:** Emitting events (like `AgreementFormalizedEvent`) for decoupled consumers utilizing RabbitMQ.

## Frontend Architecture

The frontend (Angular 17) is focused on providing an excellent corporate user experience:
- **Core / Shared**: Global services, authentication interceptors, and reused UI components.
- **Features**: Division by business domain (`admin` for logs and management, `dashboard` for billet visualization and formalization).
- **Material Design**: Extensive usage of Angular Material panels, lists, and dialogs, with a complete 100% integrated **Dark Mode** support.

## API -> Frontend Communication

The security and efficiency in communication consist of:
- **Nginx Reverse Proxy**: Acts on port 8081 providing passthrough from `/api` directly to the backend container.
- **HttpOnly Cookies**: Usage of JWT via sealed cookies against XSS attacks, aside from stringent SameSite configuration.
