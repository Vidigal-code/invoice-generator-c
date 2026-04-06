# Getting Started

Welcome to the **Invoice Generator C** comprehensive official documentation. 

This platform represents a robust, cloud-native enterprise solution constructed to manage high-throughput debt calculation, multi-tenant interactions, and the strict formalization of contracts and billets. Utilizing the `.NET 8` framework integrated with `Angular 17`, it bridges deep operational workflows with high usability.

## 1. System Prerequisites

The deployment map of this infrastructure relies heavily on modern container orchestration. To operate in a Local Environment, ensure you possess the exact following minimum versions:

| Software | Minimum Version | Purpose |
|------|-----------|-----------|
| **.NET SDK** | 8.0.x | Resolves backend builds and provides the `dotnet` CLI for EF Core tools. |
| **Node.js** | 18.17.x (LTS) | Used for installing NPM libraries required by the Angular application. |
| **Angular CLI** | 17.0+ | Framework management and local serving. |
| **Docker Engine** | 24.x+ | Hosts the multi-container platform. |
| **Docker Compose** | V2 | Recreates the isolated networking map including local S3 and message brokers. |

## 2. Setting Up the Ecosystem

The architecture is distributed. You can trigger the holistic setup via Docker, which provides an identical topological replica of the production environment locally.

### Cloning The Source

```bash
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c
```

### Fast-Track Build using Docker Compose

The `docker-compose.yml` mapped at the root includes the entire stack:
- **api**: High-performance Kestrel API on Port 8081.
- **frontend**: The Angular SPA served alongside Nginx bindings.
- **sqlserver**: Microsoft SQL Edge or standard server preloaded.
- **redis**: In-memory caching and locking layer.
- **rabbitmq**: Real-time event broker for decoupling background tasks.
- **localstack**: Development variant of AWS S3 mocking.

```bash
# Force a clean build with detached context
docker-compose up -d --build
```
> [!NOTE]
> The initial build process may take several minutes to download EF Core toolkits, restore NuGet packages, compile the Angular standalone components, and pull the heavy localstack images. 

### Local Services Ports

Once the orchestration finishes, you can interact directly with the corresponding interfaces:

| Service | Address | Accessibility |
|------|-----------|-------|
| Frontend App | `http://localhost:8081` | Direct User Interface |
| Swagger API UI | `http://localhost:8081/swagger` | Backend Developer interactions |
| RabbitMQ Panel | `http://localhost:15672` | Check Queues, Exchanges, Consumers |

## 3. Advanced Environment Configuration

Your standard `.env` or `appsettings.Development.json` mandates specific critical keys to prevent the application from halting or throwing unauthorized faults.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=InvoiceGenerator;User Id=sa;Password=Your_Password123;TrustServerCertificate=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "S3": {
    "Endpoint": "http://localhost:4566",
    "AccessKey": "test",
    "SecretKey": "test",
    "BucketName": "billets"
  },
  "Security": {
    "CorsOrigins": "http://localhost:8081,http://localhost:4200"
  },
  "AdminSettings": {
    "AdminEmail": "admin@system.local",
    "AdminPassword": "Admin@12345"
  }
}
```

## 4. Teardown and Resource Cleanup

Prolonged running sessions of localstack or MSSQL may eventually consume ample disk space. To forcefully clear volumes, cache artifacts, and reset databases:

```bash
docker-compose down -v --rmi local
```
> [!WARNING]
> Proceed with caution, the `-v` parameter will eradicate persistent volumetric data. This is useful for clearing broken database iterations, but will drop any newly seeded custom configurations.
