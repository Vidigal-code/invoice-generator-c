# Deployment Guide

The **Invoice Generator C** is heavily containerized, ensuring that transitioning from a local scenario into a full Cloud Production environment is as natural and fault-tolerant as possible.

## 1. Environments and Variables

Before hitting any Production builds, it is immensely essential to inject proper connection string scopes corresponding correctly toward targeted containers:
- `ConnectionStrings__DefaultConnection`: Production endpoint targeting the real SQL Database.
- `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`: Secured endpoint paths corresponding alongside RabbitMQ clusters.
- `Redis__ConnectionString`: Production-ready configurations addressing an active Redis High-Performance Service.

## 2. Docker Compose (Local & Staging)

Found structurally planted within the root domain stands our `docker-compose.yml`. During Staging intervals, one can re-execute it easily:

```bash
docker-compose -f docker-compose.yml up -d
```
It immediately boots up the cascaded service layer (Backend, Nginx Frontend, Mocks across Localstack, SQL Databases, Redis).

## 3. Production Deployment (CI/CD Recommended)

Targeting a true **Production-Grade** environment (AWS ECS, Kubernetes or Azure Container Apps/Pods), compilation rules change drastically via detachment contexts:
- **Backend**: Strict Image Creation targeted solely around `Dockerfile`.
- **Frontend (Angular)**:
    ```bash
    npm run build --configuration=production
    ```
    The generated core artifact files (the `dist/` directories) are packed neatly onto their dedicated Nginx service (built via `docker/nginx/default.conf`) rendering statics across `Port 80` whilst accurately upstreaming REST operations facing `/api`.

## 4. Entity Framework Migrations

Entity Framework core will not execute automatic migrations isolated within containers unless exclusively rigged through `Program.cs`. Ensure that the initialized backend environment container has true network reach alongside sufficient connection privileges granting full-fledged automated migrations context-driven runs via `context.Database.Migrate()`. Optionally, SQL initialization scripts, like `init.sql`, can serve effectively well.
