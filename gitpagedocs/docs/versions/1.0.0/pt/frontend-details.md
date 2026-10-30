# Detalhes Frontend

Aplicação de contratos de cobrança, cálculo de dívida, negociação de acordos e emissão de documentos de parcelas. O backend é C#/.NET, apesar do sufixo -c. Boletos e códigos gerados têm caráter demonstrativo: a formalização não registra cobrança em banco nem confirma pagamento bancário.

## Interface e estado

ASP.NET Core net9.0 com EF Core 9.0.2/SQL Server, MediatR 14.1, FluentValidation, MassTransit 8.3.1/RabbitMQ, Redis e QuestPDF 2026.2.4; Angular 19.2.19, TypeScript 5.7 e RxJS 7.8 no frontend. API, Application, Domain e Infrastructure separam HTTP, comandos/queries, entidades e adaptadores. S3 pode ser atendido pelo LocalStack. Não existe garantia automática de transação distribuída entre SQL, S3 e RabbitMQ.

Entidades centrais: User/Role, Contract/Installment, Agreement/Billet, ContractHistory e AuditLog. Validação usa FluentValidation e regras do domínio; o acesso ao contrato é compartilhado entre consulta e formalização. O armazenamento possui um adaptador de proteção AES-GCM. Não confunda criptografia de payload com anonimização de logs. O lock tem expiração fixa; deduplicação do consumer é verificação seguida de gravação, não uma prova de exactly-once. Arredondamento uniforme pode deixar diferença residual entre soma das parcelas e total negociado.

Cadastro → validação de senha/usuário → hash de senha → login → JWT e cookie HttpOnly/Secure/SameSite=Strict. O login também devolve Token no JSON. Controllers e ContractAccessService aplicam papéis e acesso ao contrato. Administração de usuários e contratos fica em AdminPanel; o catálogo HTTP está na referência.

## Fontes verificadas

- [frontend/src/app/app.routes.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/app.routes.ts)
- [frontend/src/app/core/services/api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api.service.ts)
- [frontend/src/app/core/services/api/admin-panel-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/admin-panel-api.service.ts)
- [frontend/src/app/core/services/api/auth-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/auth-api.service.ts)
- [frontend/src/app/core/services/api/contracts-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/contracts-api.service.ts)
- [frontend/src/app/core/services/api/debt-agreements-billets-api.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/api/debt-agreements-billets-api.service.ts)
- [frontend/src/app/core/services/notification-dialog.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/notification-dialog.service.ts)
- [frontend/src/app/core/services/theme.service.ts](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/frontend/src/app/core/services/theme.service.ts)

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
