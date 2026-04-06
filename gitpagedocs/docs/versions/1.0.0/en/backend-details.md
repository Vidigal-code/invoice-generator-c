# Backend Details (.NET Core)

The backend is built with **.NET 8** using Clean Architecture and Domain-Driven Design (DDD) principles.

## Architecture Layers
1. **API Layer**: Exposes REST endpoints. Contains Controllers, Middlewares (Exception Handling, Route Protection RBAC), and Program.cs configuration.
2. **Application Layer**: Contains business use cases, Application Services, DTOs (Data Transfer Objects), CQRS Handlers, and MassTransit consumers for message-broker queues.
3. **Domain Layer**: The core of the system. Contains Entities, Enums, Value Objects, and Domain Events. No external dependencies.
4. **Infrastructure Layer**: Handles external concerns. Contains EF Core `DbContext`, Repositories, Third-Party REST clients, and configurations for Database (SQL Server/PostgreSQL).

## Security & RBAC
A strict Route Protection Middleware is in place to parse user claims, validate JWT tokens, and check dynamic Module/Action permissions mapped to Roles in the database, ensuring strict multi-tenant isolation.

## Asynchronous Processing
MassTransit is utilized for robust message queuing logic, allowing for offloading tasks like email sending, invoice generation jobs, or webhook processing smoothly.

## Testing
Comprehensive testing is implemented using `xUnit` for Unit and Integration tests. Utilizing mock frameworks to isolate domain logic and ensure high test coverage.
