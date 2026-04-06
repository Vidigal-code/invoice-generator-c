# Visão Geral da Arquitetura

O **Invoice Generator C** foi construído utilizando um padrão de Clean Architecture rigoroso no Backend e Componentização modular no Frontend. Ele é desenhado para ser Cloud-Native e suporta processos assíncronos.

## Arquitetura do Backend

O backend (.NET 8) está organizado em camadas visando desacoplamento e manutenibilidade:
- **API**: Exposição dos endpoints REST e configurações do `Program.cs`. Uso de middlewares de segurança (ex: `RouteProtectionMiddleware`, `AuditLogMiddleware`).
- **Application**: Lógica orquestrada pelo padrão CQRS (usando `MediatR`). Contém Handlers, Commands e Queries detalhados.
- **Domain**: Regras de negócio essenciais, Entidades e Interfaces de repositórios.
- **Infrastructure**: Implementações concretas de Repositórios (`EF Core`), mensageria (`MassTransit` com RabbitMQ), caching e armazenamento S3 (via LocalStack).

### Principais Padrões Utilizados
- **Distributed Locking:** O `RedisDistributedLock` garante que não ocorram condições de corrida ao formalizar acordos.
- **Strategy Pattern:** Usado no `InvoiceGeneratorCDebtCalculationStrategy` para flexibilidade na maneira como os boletos ou carteiras são calculados.
- **Event-Driven:** Emissão de eventos (como `AgreementFormalizedEvent`) para consumidores desacoplados utilizando RabbitMQ.

## Arquitetura do Frontend

O frontend (Angular 17) foca em proporcionar uma excelente experiência ao usuário corporativo:
- **Core / Shared:** Serviços globais, interceptors de autenticação, e componentes UI reutilizados.
- **Features:** Divisão por domínios de negócio (`admin` para logs e gestão, `dashboard` para visualização e formalização de boletos).
- **Material Design:** Extenso uso dos painéis, listas e diálogos do Angular Material, com um robusto suporte a **Modo Escuro (Dark Mode)** 100% integrado.

## Comunicação API -> Frontend

A segurança e eficiência na comunicação consistem em:
- **Nginx Reverse Proxy:** Atua no port 8081 provendo repasse do `/api` diretamente para o container do backend.
- **HttpOnly Cookies:** Utilização de JWT via cookies blindados contra ataques XSS, além de SameSite configuration.
