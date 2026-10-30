# Segurança e Auditoria

Aplicação de contratos de cobrança, cálculo de dívida, negociação de acordos e emissão de documentos de parcelas. O backend é C#/.NET, apesar do sufixo -c. Boletos e códigos gerados têm caráter demonstrativo: a formalização não registra cobrança em banco nem confirma pagamento bancário.

## Contratos de dados

Entidades centrais: User/Role, Contract/Installment, Agreement/Billet, ContractHistory e AuditLog. Validação usa FluentValidation e regras do domínio; o acesso ao contrato é compartilhado entre consulta e formalização. O armazenamento possui um adaptador de proteção AES-GCM. Não confunda criptografia de payload com anonimização de logs. O lock tem expiração fixa; deduplicação do consumer é verificação seguida de gravação, não uma prova de exactly-once. Arredondamento uniforme pode deixar diferença residual entre soma das parcelas e total negociado.

## Fluxo e casos de falha

Cadastro → validação de senha/usuário → hash de senha → login → JWT e cookie HttpOnly/Secure/SameSite=Strict. O login também devolve Token no JSON. Controllers e ContractAccessService aplicam papéis e acesso ao contrato. Administração de usuários e contratos fica em AdminPanel; o catálogo HTTP está na referência.

Copie envexample.txt para .env e configure valores locais; Docker Compose reúne SQL Server/inicialização, API, Redis, RabbitMQ, Elasticsearch, LocalStack e Angular. As portas efetivas são as do Compose. Fora do Docker, use SDK .NET 9 e npm ci no frontend; prestart/prebuild sincronizam ambiente da raiz. Valide dotnet test, dotnet build, npm test e npm run build. Em boleto indisponível, confira PdfUrl, S3 e fallback; em dívida desatualizada, cache e consumer; em 409, a duração do lock. O workflow existente publica documentação, não esses serviços.

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
