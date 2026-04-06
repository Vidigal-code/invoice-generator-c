# Detalles del Frontend (Angular)

El frontend de Invoice Generator está desarrollado con **Angular (v18+)** y se apoya en una arquitectura robusta siguiendo las mejores prácticas.

## Arquitectura Central
- **Framework**: Angular
- **Diseño de Componentes**: Standalone Components (Componentes Independientes) y Feature-Sliced Design.
- **Estilización**: TailwindCSS y Angular Material.
- **Gestión del Estado**: Estado Reactivo / Servicios con Signals/RxJS.

## Módulos Clave
1. **Módulo Core**: Servicios Singleton, Interceptores (ej. AuthInterceptor para tokens JWT) y configuraciones generales del sistema.
2. **Módulo Shared**: Componentes reutilizables (botones, modales, barras de carga) y pipes globales.
3. **Módulos de Características (Features)**: Centrados en reglas de negocio (ej. Facturas, Configuración, Autenticación, Usuarios).

## Enrutamiento y Guardias (Guards)
El sistema utiliza "lazy-loading" para optimizar el rendimiento. Implementa `AuthGuard` y `RoleGuard` para evitar accesos no autorizados y respaldar la política estricta de Roles (RBAC).

## Red y APIs
Todas las llamadas a la API están fuertemente tipadas mediante interfaces TypeScript, reflejando perfectamente los DTOs del backend. Los HTTP Interceptors administran centralmente la gestión de errores y renovación de tokens.
