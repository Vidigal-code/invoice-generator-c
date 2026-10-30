# State Management

Application for collection contracts, debt calculation, agreement negotiation and installment documents. The backend is C#/.NET despite the -c suffix. Generated payment slips/codes are demonstrative: formalization does not register a bank charge or confirm bank payment.

## UI and state

ASP.NET Core net9.0 with EF Core 9.0.2/SQL Server, MediatR 14.1, FluentValidation, MassTransit 8.3.1/RabbitMQ, Redis and QuestPDF 2026.2.4; Angular 19.2.19, TypeScript 5.7 and RxJS 7.8 frontend. API, Application, Domain and Infrastructure separate HTTP, commands/queries, entities and adapters. LocalStack can provide S3. SQL, S3 and RabbitMQ do not automatically share a distributed transaction.

Core entities: User/Role, Contract/Installment, Agreement/Billet, ContractHistory and AuditLog. Validation uses FluentValidation/domain rules; contract access is shared by queries and formalization. Storage has an AES-GCM payload protector. Payload encryption is not log anonymization. The lock has a fixed expiry; consumer deduplication is check-then-write, not an exactly-once guarantee. Uniform rounding can leave a residual difference between installments and the negotiated total.

Registration → password/user validation → password hashing → login → JWT and HttpOnly/Secure/SameSite=Strict cookie. Login also returns Token in JSON. Controllers and ContractAccessService enforce roles and contract access. AdminPanel handles user/contract administration; the HTTP catalog is in the reference.

## Verified sources

- [frontend/src/app/app.routes.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/app.routes.ts)
- [frontend/src/app/core/services/api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api.service.ts)
- [frontend/src/app/core/services/api/admin-panel-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/admin-panel-api.service.ts)
- [frontend/src/app/core/services/api/auth-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/auth-api.service.ts)
- [frontend/src/app/core/services/api/contracts-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/contracts-api.service.ts)
- [frontend/src/app/core/services/api/debt-agreements-billets-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/debt-agreements-billets-api.service.ts)
- [frontend/src/app/core/services/notification-dialog.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/notification-dialog.service.ts)
- [frontend/src/app/core/services/theme.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/theme.service.ts)

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
