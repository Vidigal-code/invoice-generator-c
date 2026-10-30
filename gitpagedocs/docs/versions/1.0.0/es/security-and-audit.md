# Seguridad y Auditoria

Aplicación de contratos de cobro, cálculo de deuda, negociación y documentos de cuotas. El backend es C#/.NET pese al sufijo -c. Los documentos y códigos son demostrativos: formalizar no registra un cobro bancario ni confirma un pago.

## Contratos de datos

Entidades centrales: User/Role, Contract/Installment, Agreement/Billet, ContractHistory y AuditLog. FluentValidation y dominio validan; consultas y formalización comparten control de acceso. El almacenamiento tiene protección AES-GCM de payload. Eso no anonimiza logs. El bloqueo tiene caducidad fija; deduplicar mediante consulta y escritura no garantiza exactly-once. El redondeo puede dejar diferencia residual entre cuotas y total.

## Flujo y casos de fallo

Registro → validación de contraseña/usuario → hash → login → JWT y cookie HttpOnly/Secure/SameSite=Strict. Login también devuelve Token en JSON. Controllers y ContractAccessService aplican roles y acceso al contrato. AdminPanel administra usuarios/contratos; el catálogo HTTP está en la referencia.

Copie envexample.txt a .env y configure valores locales. Compose reúne SQL Server/inicialización, API, Redis, RabbitMQ, Elasticsearch, LocalStack y Angular. Use sus puertos efectivos. Fuera de Docker use SDK .NET 9 y npm ci en frontend; prestart/prebuild sincronizan el entorno raíz. Valide dotnet test/build y npm test/build. Si falta un documento revise PdfUrl/S3/fallback; si la deuda está obsoleta revise caché/consumer; ante 409 revise el lock. El workflow publica documentación, no estos servicios.

## Archivos relacionados

- [backend/Application/Commands/CreateAgreementCommandHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Commands/CreateAgreementCommandHandler.cs)
- [backend/Application/Consumers/AgreementFormalizedConsumer.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Consumers/AgreementFormalizedConsumer.cs)
- [backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs)
- [backend/Application/Services/Contracts/ContractAccessService.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Services/Contracts/ContractAccessService.cs)
- [backend/Application/Queries/GetBilletPdfQueryHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Queries/GetBilletPdfQueryHandler.cs)
- [backend/Infrastructure/Configuration/AppSettings.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Configuration/AppSettings.cs)
- [docker-compose.yml](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/docker-compose.yml)
- [backend/Infrastructure/Repositories/GenericRepository.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Repositories/GenericRepository.cs)

## Navegación

- [Project guide](?menu=es&lang=es&id=19)
- [Source reference](?menu=es&lang=es&id=20)
