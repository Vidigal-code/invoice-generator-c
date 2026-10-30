# Guia de Deploy

Aplicação de contratos de cobrança, cálculo de dívida, negociação de acordos e emissão de documentos de parcelas. O backend é C#/.NET, apesar do sufixo -c. Boletos e códigos gerados têm caráter demonstrativo: a formalização não registra cobrança em banco nem confirma pagamento bancário.

## Configuração e implantação

Copie envexample.txt para .env e configure valores locais; Docker Compose reúne SQL Server/inicialização, API, Redis, RabbitMQ, Elasticsearch, LocalStack e Angular. As portas efetivas são as do Compose. Fora do Docker, use SDK .NET 9 e npm ci no frontend; prestart/prebuild sincronizam ambiente da raiz. Valide dotnet test, dotnet build, npm test e npm run build. Em boleto indisponível, confira PdfUrl, S3 e fallback; em dívida desatualizada, cache e consumer; em 409, a duração do lock. O workflow existente publica documentação, não esses serviços.

## Comandos de execução

```sh
docker compose up --build
# Validation in a separately configured test environment
dotnet test backend/InvoiceGenerator.Api.csproj
dotnet build backend/InvoiceGenerator.Api.csproj
npm --prefix frontend ci
npm --prefix frontend run build
```

Nomes extraídos apenas dos arquivos de exemplo e das referências a variáveis no código. Configure os valores localmente; credenciais privadas não fazem parte desta referência. Variáveis NEXT_PUBLIC_ e VITE_ são públicas no navegador.

| Variable | Origem |
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

## Arquivos relacionados

- [backend/Application/Commands/CreateAgreementCommandHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Commands/CreateAgreementCommandHandler.cs)
- [backend/Application/Consumers/AgreementFormalizedConsumer.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Consumers/AgreementFormalizedConsumer.cs)
- [backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs)
- [backend/Application/Services/Contracts/ContractAccessService.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Services/Contracts/ContractAccessService.cs)
- [backend/Application/Queries/GetBilletPdfQueryHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Queries/GetBilletPdfQueryHandler.cs)
- [backend/Infrastructure/Configuration/AppSettings.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Configuration/AppSettings.cs)
- [docker-compose.yml](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/docker-compose.yml)
- [backend/Infrastructure/Repositories/GenericRepository.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Repositories/GenericRepository.cs)

## Navegação

- [Project guide](?menu=pt&lang=pt&id=19)
- [Source reference](?menu=pt&lang=pt&id=20)
