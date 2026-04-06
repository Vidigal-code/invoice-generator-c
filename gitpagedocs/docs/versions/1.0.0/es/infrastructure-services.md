# Servicios de Infraestructura

La capa de **Infraestructura** en el proyecto Invoice Generator C# se centra en aislar dependencias externas de la lógica de dominio principal.

## Acceso a Datos
Entity Framework Core es el ORM principal.
- **Repositorios**: Empleamos el patrón de repositorio genérico junto con interfaces específicas para encapsular el `DbContext`, evitando un fuerte acoplamiento en los controladores o servicios.

## Almacenamiento en Caché
- **Caché Redis**: Se integra una capa de caché distribuida para garantizar el rendimiento y la escalabilidad ante consultas frecuentes o pesadas, protegiendo así la base de datos SQL principal.

## Integraciones Externas
- **Servicios Rest**: Apoyándose en patrones tipados de `HttpClient`, enlazamos conexiones con proveedores fintech externos (como SocRestService) de forma fiable. Las llamadas frecuentemente se blindan con lógicas de reintento/circuit-breaker (vía Polly) de ser necesario.
