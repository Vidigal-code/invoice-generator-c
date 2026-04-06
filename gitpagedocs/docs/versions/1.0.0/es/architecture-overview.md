# Visión General de la Arquitectura

**Invoice Generator C** ha sido construido utilizando un estricto patrón de Clean Architecture en el Backend y una componentización modular en el Frontend. Está diseñado para ser Cloud-Native y soportar procesamiento asíncrono.

## Arquitectura del Backend

El backend (.NET 8) está organizado en capas con el objetivo de lograr desacoplamiento y mejor mantenimiento:
- **API**: Exposición de los endpoints REST y configuraciones del `Program.cs`. Uso de middlewares de seguridad (ej: `RouteProtectionMiddleware`, `AuditLogMiddleware`).
- **Application**: Lógica orquestada por el patrón CQRS (utilizando `MediatR`). Contiene Handlers, Commands y Queries detallados.
- **Domain**: Reglas de negocio esenciales, Entidades e Interfaces de repositorios.
- **Infrastructure**: Implementaciones concretas de los Repositorios (`EF Core`), mensajería (`MassTransit` con RabbitMQ), caching y almacenamiento S3 (vía LocalStack).

### Patrones Principales Utilizados
- **Distributed Locking:** El `RedisDistributedLock` garantiza que no ocurran condiciones de carrera al formalizar acuerdos.
- **Strategy Pattern:** Utilizado en `InvoiceGeneratorCDebtCalculationStrategy` para ofrecer flexibilidad en el cálculo de los boletos o la cartera de deudas.
- **Event-Driven:** Emisión de eventos (como `AgreementFormalizedEvent`) para los consumidores desacoplados usando RabbitMQ.

## Arquitectura del Frontend

El frontend (Angular 17) se centra en proporcionar una excelente experiencia de usuario corporativa:
- **Core / Shared**: Servicios globales, interceptores de autenticación y componentes UI reutilizados.
- **Features**: División por dominios de negocio (`admin` para logs y gestión, `dashboard` para la visualización y formalización de boletos).
- **Material Design**: Amplio uso de paneles, listas y cuadros de diálogo de Angular Material, con un robusto soporte de **Modo Oscuro (Dark Mode)** 100% integrado.

## Comunicación API -> Frontend

La seguridad y eficiencia en la comunicación se dividen en:
- **Nginx Reverse Proxy**: Actúa en el puerto 8081 proporcionando enrutamiento del `/api` directamente hacia el contenedor del backend.
- **HttpOnly Cookies**: Uso de JWT en cookies seguras contra ataques XSS, además de la sólida configuración de SameSite.
