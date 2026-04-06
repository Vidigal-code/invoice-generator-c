# Backend API - invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

This is the **ASP.NET Core 9** API for **invoice-generator-c**. The codebase follows **Clean Architecture** boundaries (physical folders mirror layers), **DDD-style** aggregates/entities in **Domain**, and **CQRS** in **Application** using **MediatR**. **FluentValidation** runs through a MediatR **pipeline behavior** before handlers execute.

### Layer map

- **`API/`** — MVC controllers (`Auth`, `Contracts`, `Debt`, `Agreements`, `Billets`, `AdminPanel`), **`ApiResponseFilter`**, **`OpenApi/`** (English + pt-BR copy, `IDocumentFilter`, cookie-auth `IOperationFilter`, `/docs` landing HTML), middleware (`ExceptionMiddleware`, `SecurityHeadersMiddleware`, `AuditLogMiddleware`, `DatadogDummyMiddleware`), correlation id propagation, PII masking on logged request bodies for non-GET traffic.  
- **`Application/`** — commands/queries/handlers, validators, auth and contract services, debt calculation strategy, billet PDF/HTML/barcode services, admin panel service, **`AuditService`** (implements `IAuditService`; encrypts IP for structured audit writes).  
- **`Domain/`** — entities, enums, repository and unit-of-work **interfaces**, CPF validation.  
- **`Infrastructure/`** — `AppDbContext`, generic repository + **unit of work**, **Redis** distributed lock, **S3** file storage + **AES-GCM** payload protection, **`AppSettings`** binding.  
- **`Tests/`** — **xUnit** + **Moq**: domain, application, infrastructure, and **WebApplicationFactory** integration tests (in-memory EF when `ASPNETCORE_ENVIRONMENT=IntegrationTests`).

### Runtime wiring (`Program.cs`)

- **EF Core** → SQL Server (or in-memory for integration tests).  
- **StackExchange.Redis** cache + **`IConnectionMultiplexer`** for locks.  
- **MassTransit** over **RabbitMQ** (e.g. **`AgreementFormalizedConsumer`**).  
- **Authentication** — JWT validation; bearer token supplied from **`AuthToken`** cookie; **JWE** decryption via configured secret.  
- **Authorization** — role policies (e.g. formalize agreements).  
- **Observability** — **Serilog** to console + rolling file; optional **Elasticsearch** sink if `ElasticSearch:Uri` is set.  
- **Resilience** — named **`HttpClient`** with **Polly** standard handler.  
- **API docs** — **Swashbuckle** generates **`v1-en`** and **`v1-br`** with localized summaries/tags; **Swagger UI** at `/docs/en/swagger` and `/docs/br/swagger`; **ReDoc** (`Swashbuckle.AspNetCore.ReDoc`) at `/docs/en/redoc` and `/docs/br/redoc`; hub **`/docs`**; JSON at `/swagger/v1-en/swagger.json` and `/swagger/v1-br/swagger.json`. **Microsoft.AspNetCore.OpenAPI** `MapOpenApi()` remains. XML comments still enrich schemas. **SecurityHeadersMiddleware** skips strict CSP for `/swagger`, `/docs`, and `/openapi`.  
- **Database** — **`EnsureCreatedAsync`** at startup + role/admin seed + optional **`ContractDemoSeed`** (skipped in integration test environment).

### How to run

From the repository root, **`docker compose up --build`** starts SQL Server (with **`docker/sql/init.sql`** via **`db-init`**), Redis, RabbitMQ, Elasticsearch, LocalStack, the **api** service, and the **frontend** container. For local **`dotnet run`**, set connection strings and secrets to match **`appsettings.Development.json`** / environment variables (Redis, RabbitMQ, JWT/JWE, CORS, logging path, etc.).

**Interactive docs in Docker:** open **`http://localhost:<BACKEND_HOST_PORT>/docs`** (default **5283** unless overridden). In Swagger, call **`POST /api/Auth/login`** first so the browser stores the **`AuthToken`** cookie for protected endpoints.

To run tests in Docker: **`docker compose --profile test up`** (see compose file for **`backend-test`**).

</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

API **ASP.NET Core 9** do **invoice-generator-c**, organizada em **Clean Architecture** (pastas **API**, **Application**, **Domain**, **Infrastructure**) com **CQRS** via **MediatR** e validação com **FluentValidation** (comportamento em pipeline).

### Camadas

- **`API/`** — Controllers, filtro de resposta, pasta **`OpenApi/`** (textos EN / pt-BR, filtros Swashbuckle, página **`/docs`**), middlewares (exceções, cabeçalhos, auditoria com **mascaramento de PII**).  
- **`Application/`** — comandos/consultas, serviços (autenticação, contratos, dívidas, boletos, admin), **`AuditService`**.  
- **`Domain/`** — entidades, interfaces de repositório e **unit of work**, validações de domínio.  
- **`Infrastructure/`** — **EF Core**, **Redis**, armazenamento **S3**, proteção de ficheiros.  
- **`Tests/`** — **xUnit** / **Moq** e testes de integração com **`WebApplicationFactory`**.

### Arranque

**Serilog** (consola + ficheiro; **Elasticsearch** opcional), **JWT** em cookie **HttpOnly** com **JWE**, **MassTransit** + **RabbitMQ**, **Redis**, limitação de taxa global, **documentação OpenAPI em inglês e pt-BR** (**Swagger UI** + **ReDoc**, índice **`/docs`**). Esquema criado com **`EnsureCreatedAsync`** (sem migrações EF versionadas no repositório).

### Execução

Na raiz do monorepo: **`docker compose up --build`**. Testes: perfil **`test`** no Compose. Para desenvolvimento local, configurar variáveis alinhadas com **`appsettings.Development.json`**.

</details>
