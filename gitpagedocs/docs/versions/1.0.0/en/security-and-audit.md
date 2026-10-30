# Security & Audit

Application for collection contracts, debt calculation, agreement negotiation and installment documents. The backend is C#/.NET despite the -c suffix. Generated payment slips/codes are demonstrative: formalization does not register a bank charge or confirm bank payment.

## Data contracts

Core entities: User/Role, Contract/Installment, Agreement/Billet, ContractHistory and AuditLog. Validation uses FluentValidation/domain rules; contract access is shared by queries and formalization. Storage has an AES-GCM payload protector. Payload encryption is not log anonymization. The lock has a fixed expiry; consumer deduplication is check-then-write, not an exactly-once guarantee. Uniform rounding can leave a residual difference between installments and the negotiated total.

## Workflow and failure cases

Registration → password/user validation → password hashing → login → JWT and HttpOnly/Secure/SameSite=Strict cookie. Login also returns Token in JSON. Controllers and ContractAccessService enforce roles and contract access. AdminPanel handles user/contract administration; the HTTP catalog is in the reference.

Copy envexample.txt to .env and configure local values. Docker Compose includes SQL Server/bootstrap, API, Redis, RabbitMQ, Elasticsearch, LocalStack and Angular. Effective ports come from Compose. Outside Docker, use .NET 9 SDK and frontend npm ci; prestart/prebuild synchronize root environment. Validate dotnet test, dotnet build, npm test and npm run build. For missing slips check PdfUrl/S3/fallback; stale debt values require cache/consumer checks; 409 requires lock inspection. The existing workflow publishes documentation, not these services.

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
