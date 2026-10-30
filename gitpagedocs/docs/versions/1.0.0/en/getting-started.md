# Getting Started

Application for collection contracts, debt calculation, agreement negotiation and installment documents. The backend is C#/.NET despite the -c suffix. Generated payment slips/codes are demonstrative: formalization does not register a bank charge or confirm bank payment.

## Configuration and deployment

Copy envexample.txt to .env and configure local values. Docker Compose includes SQL Server/bootstrap, API, Redis, RabbitMQ, Elasticsearch, LocalStack and Angular. Effective ports come from Compose. Outside Docker, use .NET 9 SDK and frontend npm ci; prestart/prebuild synchronize root environment. Validate dotnet test, dotnet build, npm test and npm run build. For missing slips check PdfUrl/S3/fallback; stale debt values require cache/consumer checks; 409 requires lock inspection. The existing workflow publishes documentation, not these services.

## Execution commands

```sh
docker compose up --build
# Validation in a separately configured test environment
dotnet test backend/InvoiceGenerator.Api.csproj
dotnet build backend/InvoiceGenerator.Api.csproj
npm --prefix frontend ci
npm --prefix frontend run build
```

Names are extracted only from example files and environment references in source. Configure values locally; private credentials are excluded. NEXT_PUBLIC_ and VITE_ variables are public in the browser.

| Variable | Source |
| --- | --- |
| `ADMIN_EMAIL` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `ADMIN_PASSWORD` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `ADMIN_USERNAME` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `ANGULAR_API_URL` | [frontend/scripts/sync-env-from-root.mjs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/scripts/sync-env-from-root.mjs) |
| `ANGULAR_APP_TITLE` | [frontend/scripts/sync-env-from-root.mjs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/scripts/sync-env-from-root.mjs) |
| `API_TITLE` | [frontend/scripts/sync-env-from-root.mjs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/scripts/sync-env-from-root.mjs) |
| `AWS_ACCESS_KEY_ID` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `AWS_ENDPOINT_URL` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `AWS_REGION` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `AWS_S3_BUCKET` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `AWS_SECRET_ACCESS_KEY` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `BACKEND_CONTAINER_PORT` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `BACKEND_HOST_PORT` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `CORS_ALLOWED_ORIGINS` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `DB_PASSWORD` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `ELASTICSEARCH_URI` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `ENVIRONMENT` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `FRONTEND_CONTAINER_PORT` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `FRONTEND_HOST_PORT` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `JWE_SECRET` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `JWT_SECRET` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `LOGGING_FILE_PATH` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `LOGGING_LEVEL` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RABBITMQ_HOST` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RABBITMQ_PASS` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RABBITMQ_USER` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RATE_LIMIT_PERMITS` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RATE_LIMIT_QUEUE` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `RATE_LIMIT_WINDOW_MINUTES` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `REDIS_CONNECTION` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `SECURITY_MAX_PAYLOAD_MB` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `SECURITY_REQUEST_TIMEOUT_SECONDS` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |
| `START_THEME` | [envexample.txt](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/envexample.txt) |

## Related files

- [backend/Application/Commands/CreateAgreementCommandHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Commands/CreateAgreementCommandHandler.cs)
- [backend/Application/Consumers/AgreementFormalizedConsumer.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Consumers/AgreementFormalizedConsumer.cs)
- [backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs)
- [backend/Application/Services/Contracts/ContractAccessService.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Services/Contracts/ContractAccessService.cs)
- [backend/Application/Queries/GetBilletPdfQueryHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Queries/GetBilletPdfQueryHandler.cs)
- [backend/Infrastructure/Configuration/AppSettings.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Configuration/AppSettings.cs)
- [docker-compose.yml](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/docker-compose.yml)
- [backend/Infrastructure/Repositories/GenericRepository.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Repositories/GenericRepository.cs)

## Navigation

- [Project guide](?menu=en&lang=en&id=19)
- [Source reference](?menu=en&lang=en&id=20)
