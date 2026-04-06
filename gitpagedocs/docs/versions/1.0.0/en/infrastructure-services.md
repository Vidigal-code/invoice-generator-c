# Infrastructure Services

The **Infrastructure** layer in the Invoice Generator C# project focuses on isolating external dependencies away from the core domain.

## Data Access
Entity Framework Core is the primary ORM. 
- **Repositories**: We use the generic repository pattern combined with specific interfaces to wrap `DbContext`, avoiding tight coupling in controllers or services.

## Caching
- **Redis Cache**: A distributed caching layer is integrated to ensure repeated, heavy data queries scale well without degrading the primary SQL database performance.

## External Integrations
- **Rest Services**: Leveraging typed `HttpClient` patterns, we establish connections with external fintech providers (such as SocRestService) reliably, wrapping calls with Polly resilient retry/circuit-breaker logic when necessary.
