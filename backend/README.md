# Backend API - invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

This is the core backend API for the **invoice-generator-c** portfolio management system. It embraces **Clean Architecture**, **Domain-Driven Design (DDD)**, and the **CQRS** pattern using `MediatR` to ensure a strict separation of concerns and adherence to **SOLID** principles.

Key features include:
- **Enterprise-Grade Security:** Implementing token protection (JWE/JWT) embedded inside HttpOnly cookies to thwart XSS attacks and Session Hijacking.
- **Anti-DDoS:** Active ASP.NET Core `RateLimiting` on global endpoints.
- **Traceability:** Automatic HTTP traffic interceptions handled by the `AuditLogMiddleware` tracking user IPs and durations.
- **High Performance:** Designed to run blazingly fast using EF Core targeting SQL Server. Uses rigorous Rollback mechanisms via the `IUnitOfWork` repository abstraction.

**To Run:**
`docker compose up --build` will lift up the SQL Server instance along with the automated DB migrations and this `.NET 9` app.
</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

Este é o Backend primário para gestão da carteira do **invoice-generator-c**, desenvolvido em C# sob os paradigmas de **Clean Architecture**, **DDD (Domain-Driven Design)** e **CQRS** via `MediatR`. Tudo foi particionado em funções puras, garantindo que o código obedeça os princípios **SOLID** (Responsabilidade Única).

Principais características corporativas (Padrão Fintech):
- **Swagger Dual Bilingue:** A UI do Swagger nativo (route `/docs`) é fracionada em 2 repositórios interativos, varrendo os Comentários XML JSDoc do C# automaticamente para montar o Swagger UI nas versões `/docs/pt` e `/docs/en`.
- **Matriz de Rastreabilidade Total:** Integração ponta-a-ponta de `X-Correlation-ID` com **Masking Automático de PII** (DLP - Data Loss Prevention) via `AuditLogMiddleware` - impossibilitando vazamento de credenciais na Infraestrutura e atrelando requisições a ciclos únicos no banco de dados.
- **Segurança Sênior:** Defesas contra Roubo de Sessão adotando Cookies HttpOnly, isolando a API e bloqueando ataques usando injeção profunda de `HSTS`, `Referrer-Policy` strict, e `Content-Security-Policy`.
- **Prevenção DDOS:** Configuração agressiva de Rate Limit Particionado visando suportar e isolar explosões de tráfego.

**Para Iniciar:**
`docker compose up --build` executa instantaneamente o container com setup finalizado.
</details>
