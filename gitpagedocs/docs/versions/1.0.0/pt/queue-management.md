# Mensageria e Filas

Aplicação de contratos de cobrança, cálculo de dívida, negociação de acordos e emissão de documentos de parcelas. O backend é C#/.NET, apesar do sufixo -c. Boletos e códigos gerados têm caráter demonstrativo: a formalização não registra cobrança em banco nem confirma pagamento bancário.

## Fluxo e casos de falha

1. Cadastro → validação de senha/usuário → hash de senha → login → JWT e cookie HttpOnly/Secure/SameSite=Strict. O login também devolve Token no JSON. Controllers e ContractAccessService aplicam papéis e acesso ao contrato. Administração de usuários e contratos fica em AdminPanel; o catálogo HTTP está na referência.

2. Contrato → parcelas → cálculo por carteira. Para cada parcela, diasEmAtraso=max(0, dataReferência-dataVencimento); multa=principal×percentual/100 apenas se atrasada; juros=principal×(taxaMensal/100/diasPorMês)×diasEmAtraso. As taxas vêm de AppSettings; diasPorMês recai em 30 quando inválido. CurrentValue soma principal, multa e juros. A query usa cache e respeita acesso ao contrato.

3. Formalização valida de 1 a 24 parcelas, adquire Redis formalize:{contractId} por 45 segundos, verifica acesso e acordo ativo. Trava indisponível retorna 409; acordo ativo retorna 400. Gera Agreement e Billets, divide o valor com arredondamento a duas casas e vencimentos em intervalos de 30 dias. QuestPDF gera PDFs no próprio handler, antes da publicação do evento.

4. Cada PDF segue ao adaptador de armazenamento; se o upload falha, o handler grava uma URL mock. O contrato muda para Negotiated e publica AgreementFormalizedEvent. O consumer invalida o cache de dívida e grava idem:agreement:{id} no Redis por 30 dias; ele não gera os PDFs. A consulta de boleto verifica acesso e tenta recuperar o PDF, com fallback HTML conforme o serviço.

5. GenericRepository.AddAsync e UpdateAsync chamam SaveChangesAsync imediatamente. Agreement é persistido antes do loop de PDFs, cada Billet após seu upload e o contrato antes do evento. Não há transação envolvendo a formalização inteira; uma falha intermediária pode deixar acordo ou parcelas parciais. Confira o estado persistido antes de tentar novamente.

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
