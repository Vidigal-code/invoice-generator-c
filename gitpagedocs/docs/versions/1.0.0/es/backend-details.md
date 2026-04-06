# Detalles del Backend (.NET Core)

El backend está construido con **.NET 8** utilizando Arquitectura Limpia (Clean Architecture) y principios de Diseño Guiado por el Dominio (DDD).

## Capas de la Arquitectura
1. **Capa de API**: Expone endpoints REST. Contiene Controladores, Middlewares (Manejo de Excepciones, Protección de Rutas RBAC) y la configuración de Program.cs.
2. **Capa de Aplicación**: Contiene los casos de uso, Servicios de Aplicación, DTOs, Handlers (CQRS) y consumidores de MassTransit para colas de mensajería.
3. **Capa de Dominio**: El núcleo del sistema. Contiene Entidades, Enums, Value Objects y Eventos de Dominio. No tiene dependencias externas.
4. **Capa de Infraestructura**: Maneja las interacciones externas. Contiene el `DbContext` de EF Core, Repositorios, clientes REST de terceros y configuración de bases de datos.

## Seguridad y RBAC
Cuenta con un estricto Middleware de Protección de Rutas para analizar claims del usuario, validar tokens JWT y revisar permisos dinámicos de Módulo/Acción mapeados a Roles en la base de datos, garantizando aislamiento multi-tenant.

## Procesamiento Asíncrono
MassTransit se utiliza para gestionar colas de mensajes de forma sólida, permitiendo la delegación de tareas en segundo plano como envío de correos, generación de facturas o procesamiento de webhooks.

## Pruebas
Pruebas exhaustivas mediante Pruebas Unitarias y de Integración con `xUnit`, usando frameworks de mock para aislar la lógica del dominio y asegurar una alta calidad de software.
