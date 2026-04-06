# Frontend Details (Angular)

The frontend of Invoice Generator is built with **Angular (v18+)** and relies on a robust architecture following best practices.

## Core Architecture
- **Framework**: Angular
- **Component Design**: Standalone Components and Feature-Sliced Design.
- **Styling**: TailwindCSS and Angular Material.
- **State Management**: Reactive State / Services.

## Key Modules
1. **Core Module**: Singleton services, interceptors (e.g., AuthInterceptor for JWT tokens), and application-wide configurations.
2. **Shared Module**: Reusable components (buttons, dialogs, spinners) and pipes.
3. **Features Modules**: Focused on business rules (e.g., Invoices, Settings, Authentication, Users).

## Routing and Guards
The routing system is lazy-loaded for performance optimization. It uses `AuthGuard` and `RoleGuard` to prevent unauthorized access and implement RBAC policies dynamically.

## Networking
All API calls are strictly typed through TypeScript interfaces representing the DTOs from the backend. HTTP Interceptors handle centralized error catches and token refreshes.
