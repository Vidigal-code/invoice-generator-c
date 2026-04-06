# invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

Welcome to **invoice-generator-c**, an enterprise-style **debt simulation and agreement formalization** white-label platform. The repo is a **full-stack monorepo**: **ASP.NET Core 9** API, **Angular 19** SPA, and **SQL Server**, orchestrated with **Docker Compose** so services stay isolated in containers.

### Architecture overview

**Backend (`/backend`)** — logical **Clean Architecture** split into physical folders:

| Layer | Responsibility |
|--------|----------------|
| **API** | HTTP controllers, filters (`ApiResponseFilter`), middleware (exceptions, security headers, audit, rate limiting helpers), `HttpCurrentUserAccessor` |
| **Application** | CQRS with **MediatR** (commands/queries/handlers), **FluentValidation** (+ pipeline `ValidationBehavior`), domain services (auth, contracts, admin, billets, debt strategies), DTOs, MassTransit **consumer** (`AgreementFormalizedConsumer`) |
| **Domain** | Entities (users, contracts, agreements, billets, audit, etc.), value rules (e.g. CPF), role names |
| **Infrastructure** | **EF Core** + SQL Server (`AppDbContext`, repositories, **unit of work**), **Redis** (distributed cache + **distributed locks**), **S3-compatible** storage (encrypted payload wrapper + AES-GCM protector), configuration binding |

Cross-cutting in **`Program.cs`**: **Serilog** (rolling file; optional **Elasticsearch** sink), **JWT Bearer** with token read from **HttpOnly** `AuthToken` cookie and **JWE** decryption key, global **rate limiting**, request timeouts, Kestrel body limits, **CORS** with credentials, **Polly** resilience on `HttpClient`, **OpenAPI** (`MapOpenApi`), and **dual-language API reference** in Development/Production: OpenAPI **`v1-en`** (English) and **`v1-br`** (Brazilian Portuguese), each with **Swagger UI** and **ReDoc**, plus hub **`/docs`**. CSP is relaxed on `/docs`, `/swagger`, and `/openapi` so bundled documentation UIs load. On startup the API uses **`Database.EnsureCreatedAsync`** (no checked-in migrations) plus optional **demo contract seeding**; the **`db-init`** service runs **`docker/sql/init.sql`** against SQL Server.

### API documentation (same host as the API — works in Docker)

| Resource | Path |
|----------|------|
| Hub | `/docs` |
| Swagger UI (English) | `/docs/en/swagger` |
| Swagger UI (pt-BR) | `/docs/br/swagger` |
| ReDoc (English) | `/docs/en/redoc` |
| ReDoc (pt-BR) | `/docs/br/redoc` |
| OpenAPI JSON | `/swagger/v1-en/swagger.json`, `/swagger/v1-br/swagger.json` |

Use the **`api`** service port from Compose (e.g. **`http://localhost:5283`** if `BACKEND_HOST_PORT` is default).

**Frontend (`/frontend`)** — **Angular 19** (standalone components, **Angular Material**). Layout:

- **`core/`** — guards, interceptors (JWT, credentials + **`X-Correlation-ID`**), API services, validators, date adapter, constants  
- **`features/`** — `home`, `auth` (login/register), `dashboard` (incl. billet viewer), `admin` (logs, dialogs)  
- **`shared/`** — layout shell, navbar, footer, reusable UI  
- **`state/`** — shared application state  

**Docker Compose (`docker-compose.yml`)** — typical stack:

- **sqlserver** + **db-init** (SQL bootstrap from `./docker/sql`)  
- **api** (backend image)  
- **frontend** (nginx-served Angular build)  
- **redis**, **rabbitmq** (MassTransit), **elasticsearch** (optional logs), **localstack** (S3 API for local/dev)  

Optional **TDD profiles**: `docker compose --profile test up` runs **backend-test** (`dotnet test`) and **frontend-test** (`ng test` headless).

**Quick start:** configure root **`.env`** (see **`envexample.txt`**), then:

```bash
docker compose up --build
```

</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

Bem-vindo ao **invoice-generator-c**, plataforma **white-label** para **simulação de dívidas** e **formalização de acordos**. O repositório é um **monorepo full-stack**: API **ASP.NET Core 9**, SPA **Angular 19** e **SQL Server**, orquestrados com **Docker Compose**.

### Visão da arquitetura

**Backend (`/backend`)** — **Clean Architecture** refletida em pastas:

| Camada | Função |
|--------|--------|
| **API** | Controllers HTTP, filtros, middlewares (erro, headers, auditoria, rate limit), acesso ao utilizador atual |
| **Application** | **CQRS** com **MediatR**, **FluentValidation**, serviços de aplicação, DTOs, consumidor **MassTransit** para eventos de acordo |
| **Domain** | Entidades, regras (ex.: CPF), papéis |
| **Infrastructure** | **EF Core** + SQL Server, **Redis** (cache e locks), armazenamento estilo **S3** com camada de encriptação |

O **`Program.cs`** integra **Serilog** (ficheiro; opcional **Elasticsearch**), **JWT** em cookie **HttpOnly** com **JWE**, **rate limiting**, timeouts, **CORS**, **Polly**, **OpenAPI** e **documentação em inglês e pt-BR** (`v1-en`, `v1-br`): **Swagger UI** e **ReDoc**, índice **`/docs`**, ficheiros em **`API/OpenApi/`**, CSP flexível em `/docs` e `/swagger`. Base de dados com **`EnsureCreatedAsync`**; **`db-init`** executa **`docker/sql/init.sql`**.

### Documentação da API (Docker)

| Recurso | Caminho |
|---------|---------|
| Índice | `/docs` |
| Swagger (inglês) | `/docs/en/swagger` |
| Swagger (pt-BR) | `/docs/br/swagger` |
| ReDoc (inglês) | `/docs/en/redoc` |
| ReDoc (pt-BR) | `/docs/br/redoc` |
| JSON OpenAPI | `/swagger/v1-en/swagger.json`, `/swagger/v1-br/swagger.json` |

Porta do serviço **`api`** no Compose (ex.: **`http://localhost:5283`**).

**Frontend (`/frontend`)** — **Angular 19**, componentes **standalone** e **Material**. Estrutura: **`core/`** (guards, interceptors com **`X-Correlation-ID`**, serviços HTTP), **`features/`** (home, auth, dashboard, admin), **`shared/`**, **`state/`**.

**Docker Compose** inclui **SQL Server**, **API**, **frontend**, **Redis**, **RabbitMQ**, **Elasticsearch** e **LocalStack** (S3). Perfil **`test`** para **xUnit** e **Karma** em CI local.

**Início rápido:** configurar **`.env`** na raiz (ver **`envexample.txt`**), depois:

```bash
docker compose up --build
```

</details>
