# Local Environments & Compose

To ensure frictionless onboarding into the Invoice Generator project, we use Docker.

## Running Locally
Run `docker-compose up -d`. This automatically instances:
1. `SQL Server` for EF Core.
2. `Redis` for caching dependencies.
3. `RabbitMQ` for MassTransit events locally.
