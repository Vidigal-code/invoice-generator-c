# Entidades de Domínio

Aplicação de contratos de cobrança, cálculo de dívida, negociação de acordos e emissão de documentos de parcelas. O backend é C#/.NET, apesar do sufixo -c. Boletos e códigos gerados têm caráter demonstrativo: a formalização não registra cobrança em banco nem confirma pagamento bancário.

## Contratos de dados

Entidades centrais: User/Role, Contract/Installment, Agreement/Billet, ContractHistory e AuditLog. Validação usa FluentValidation e regras do domínio; o acesso ao contrato é compartilhado entre consulta e formalização. O armazenamento possui um adaptador de proteção AES-GCM. Não confunda criptografia de payload com anonimização de logs. O lock tem expiração fixa; deduplicação do consumer é verificação seguida de gravação, não uma prova de exactly-once. Arredondamento uniforme pode deixar diferença residual entre soma das parcelas e total negociado.

### Agreement

[backend/Domain/Entities/Agreement.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/Agreement.cs)

| Field | Type |
| --- | --- |
| `ContractId` | `Guid` |
| `TotalNegotiatedValue` | `decimal` |
| `InstallmentsCount` | `int` |
| `Status` | `InvoiceGenerator.Api.Domain.Enums.AgreementStatus` |
| `Contract` | `Contract?` |
| `Billets` | `ICollection<Billet>` |

### AuditLog

[backend/Domain/Entities/AuditLog.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/AuditLog.cs)

| Field | Type |
| --- | --- |
| `UserId` | `Guid?` |
| `Action` | `string` |
| `EntityName` | `string` |
| `EntityId` | `string?` |
| `OldValues` | `string?` |
| `NewValues` | `string?` |
| `IpAddress` | `string?` |

### BaseEntity

[backend/Domain/Entities/BaseEntity.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/BaseEntity.cs)

| Field | Type |
| --- | --- |
| `Id` | `Guid` |
| `CreatedAt` | `DateTime` |
| `UpdatedAt` | `DateTime?` |

### Billet

[backend/Domain/Entities/Billet.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/Billet.cs)

| Field | Type |
| --- | --- |
| `AgreementId` | `Guid` |
| `InstallmentNumber` | `int` |
| `DueDate` | `DateTime` |
| `Value` | `decimal` |
| `Barcode` | `string` |
| `PdfUrl` | `string?` |
| `Status` | `InvoiceGenerator.Api.Domain.Enums.BilletStatus` |
| `Agreement` | `Agreement?` |

### Contract

[backend/Domain/Entities/Contract.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/Contract.cs)

| Field | Type |
| --- | --- |
| `OwnerUserId` | `Guid?` |
| `OwnerUser` | `User?` |
| `ContractNumber` | `string` |
| `DebtorName` | `string` |
| `DebtorDocument` | `string` |
| `OriginalValue` | `decimal` |
| `CurrentBalance` | `decimal` |
| `Portfolio` | `InvoiceGenerator.Api.Domain.Enums.WalletPortfolio` |
| `Status` | `InvoiceGenerator.Api.Domain.Enums.ContractStatus` |
| `Installments` | `ICollection<Installment>` |
| `Agreements` | `ICollection<Agreement>` |

### ContractHistory

[backend/Domain/Entities/ContractHistory.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/ContractHistory.cs)

| Field | Type |
| --- | --- |
| `ContractId` | `Guid` |
| `ChangedByUserId` | `Guid?` |
| `ChangeType` | `string` |
| `PayloadJson` | `string` |
| `Contract` | `Contract?` |

### Installment

[backend/Domain/Entities/Installment.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/Installment.cs)

| Field | Type |
| --- | --- |
| `ContractId` | `Guid` |
| `InstallmentNumber` | `int` |
| `DueDate` | `DateTime` |
| `OriginalValue` | `decimal` |
| `CurrentValue` | `decimal` |
| `Status` | `InvoiceGenerator.Api.Domain.Enums.InstallmentStatus` |
| `Contract` | `Contract?` |

### Role

[backend/Domain/Entities/Role.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/Role.cs)

| Field | Type |
| --- | --- |
| `Name` | `string` |
| `Description` | `string?` |
| `Users` | `ICollection<User>` |

### User

[backend/Domain/Entities/User.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Domain/Entities/User.cs)

| Field | Type |
| --- | --- |
| `RoleId` | `Guid` |
| `Username` | `string` |
| `PasswordHash` | `string` |
| `Email` | `string` |
| `IsActive` | `bool` |
| `Role` | `Role?` |
| `OwnedContracts` | `ICollection<Contract>` |

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
