# Regras de Negócio

O sistema foi desenhado em torno de domínios corporativos rigorosos e fluxos seguros de formalização. Aqui detalhamos as regras de negócio sob a ótica do sistema **Invoice Generator C**.

## 1. Gestão e Cálculo de Dívidas

Toda vez que o usuário visualiza uma dívida (Contracts), o cálculo exato do montante passa por uma estratégia dedicada chamada `InvoiceGeneratorCDebtCalculationStrategy`.
- As dívidas podem conter encargos, juros e multas calculados em tempo real (on-the-fly) ou pré-processados.
- Contratos (Contracts) possuem uma relação de 1 para N com sub-dívidas (portfolios/faturas).

## 2. Acordos e Formalização (Agreement Formalized)

A formalização de um acordo é a ação core do sistema.
- Ao requisitar um acordo, o Backend entra no comando `CreateAgreementCommandHandler`.
- **Prevenção de Corrida (Race Conditions):** Antes de iniciar qualquer criação, o sistema dispara o `RedisDistributedLock` para a dívida/contrato assinado. Isso garante que requisições simuktâneas (ex: clique duplo do frontend) não resultem em dois boletos diferentes para a mesma dívida.
- Se o Lock for adquirido, a dívida é formalizada e um **Boleto** em formato PDF é emitido com a biblioteca `QuestPDF`.

## 3. Emissão de Boletos (Billets)

A visualização do Boleto engloba Segurança e Retenção:
- O boleto (PDF) não é devolvido via Base64 de forma insegura. Ele é criptografado e salvo no S3 LocalStack.
- Um link temporário e seguro é retornado com um IFrame sandbox (`BilletViewerComponent`) no frontend.

## 4. RBAC e Proteções de Segurança

Os Tenant e Controle de Acesso Baseado em Roles (RBAC) estão enraizados na aplicação.
- A exclusividade das permissões é validada no middleware `RouteProtectionMiddleware.cs`.
- Todo evento sensível de falha ou manipulação reflete-se como um log no **Audit Service**, onde IP é salvo com padrão pseudo-anonimizado (AES-256 IP mask).
