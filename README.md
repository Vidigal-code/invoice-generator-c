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

Cross-cutting in **`Program.cs`**: **Serilog** (rolling file; optional **Elasticsearch** sink), **JWT Bearer** with token read from **HttpOnly** `AuthToken` cookie and **JWE** decryption key, global **rate limiting**, request timeouts, Kestrel body limits, **CORS** with credentials, **Polly** resilience on `HttpClient`, **Swagger + Swagger UI** and **OpenAPI** in Development/Production. On startup the API uses **`Database.EnsureCreatedAsync`** (no checked-in migrations) plus optional **demo contract seeding**; the **`db-init`** service runs **`docker/sql/init.sql`** against SQL Server.

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

**Quick start:** configure root **`.env`** (see **`.env.example`**), then:

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

O **`Program.cs`** integra **Serilog** (ficheiro; opcional **Elasticsearch**), **JWT** lido de cookie **HttpOnly** com suporte **JWE**, **rate limiting**, timeouts, **CORS** com credenciais, **Polly**, **Swagger/Swagger UI** e **OpenAPI**. A base de dados é criada com **`EnsureCreatedAsync`** (sem migrações versionadas no repo); o serviço **`db-init`** executa **`docker/sql/init.sql`**.

**Frontend (`/frontend`)** — **Angular 19**, componentes **standalone** e **Material**. Estrutura: **`core/`** (guards, interceptors com **`X-Correlation-ID`**, serviços HTTP), **`features/`** (home, auth, dashboard, admin), **`shared/`**, **`state/`**.

**Docker Compose** inclui **SQL Server**, **API**, **frontend**, **Redis**, **RabbitMQ**, **Elasticsearch** e **LocalStack** (S3). Perfil **`test`** para **xUnit** e **Karma** em CI local.

**Início rápido:** configurar **`.env`** na raiz (ver **`.env.example`**), depois:

```bash
docker compose up --build
```

</details>
