# Primeros Pasos

Bienvenido a la documentación oficial de **Invoice Generator C**.

Esta plataforma corporativa es un sistema completo para el cálculo de deudas, formalización de acuerdos y gestión con funciones avanzadas de auditoría y seguridad.

## Requisitos previos

Para ejecutar el proyecto, necesitará:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) y Angular CLI 17+
- [Docker y Docker Compose](https://www.docker.com/)

## Instalación y Ejecución (Docker)

La forma más fácil de ejecutar el proyecto localmente con todos sus servicios dependientes (SQL Server, Redis, RabbitMQ, LocalStack S3) es mediante el uso de Docker Compose.

```bash
# Clonar el repositorio
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c

# Levantar la infraestructura completa a través de Docker
docker-compose up -d
```

Acceda a los servicios:
- **Panel Frontend**: http://localhost:8081
- **API Swagger/ReDoc**: http://localhost:8081/swagger
- **RabbitMQ Management**: http://localhost:15672

## Variables de Entorno Esenciales

Las siguientes variables configuran el entorno en `appsettings.json` o variables de Docker:
- `ConnectionStrings:DefaultConnection`: Conexión al servidor SQL.
- `Security:CorsOrigins`: Orígenes de frontend permitidos para prevenir bloqueo CORS.
- `AdminSettings:AdminEmail` / `AdminSettings:AdminPassword`: Credenciales del primer administrador en el modo Semilla del sistema.

## Scripts npm y Comandos .NET

**Backend:**
```bash
cd backend
dotnet restore
dotnet build
dotnet run --environment Development
```

**Frontend:**
```bash
cd frontend
npm install
npm start
```
