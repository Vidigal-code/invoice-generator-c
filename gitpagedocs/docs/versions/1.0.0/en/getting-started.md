# Getting Started

Welcome to the official documentation for **Invoice Generator C**.

This corporate platform is a complete system for debt calculation, agreement formalization, and management with advanced audit and security features.

## Prerequisites

To run the project, you will need:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) and Angular CLI 17+
- [Docker and Docker Compose](https://www.docker.com/)

## Installation and Execution (Docker)

The easiest way to run the project locally with all its dependent services (SQL Server, Redis, RabbitMQ, LocalStack S3) is by using Docker Compose.

```bash
# Clone the repository
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c

# Bring up the complete infrastructure via Docker
docker-compose up -d
```

Access the services:
- **Frontend Dashboard**: http://localhost:8081
- **API Swagger/ReDoc**: http://localhost:8081/swagger
- **RabbitMQ Management**: http://localhost:15672

## Essential Environment Variables

The following variables configure the environment in `appsettings.json` or Docker variables:
- `ConnectionStrings:DefaultConnection`: Connection to SQL Server.
- `Security:CorsOrigins`: Allowed frontend origins to prevent CORS block.
- `AdminSettings:AdminEmail` / `AdminSettings:AdminPassword`: Seed credentials for the first administrator.

## npm Scripts and .NET Commands

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
