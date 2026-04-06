# Detalhes do Backend (.NET Core)

O backend é construído com **.NET 8** utilizando Arquitetura Limpa (Clean Architecture) e princípios de Domain-Driven Design (DDD).

## Camadas da Arquitetura
1. **Camada de API**: Expõe os endpoints REST. Contém Controllers, Middlewares (Tratamento de Exceções, Proteção de Rotas / RBAC) e a configuração do Program.cs.
2. **Camada de Aplicação**: Contém casos de uso de negócios, Serviços de Aplicação, DTOs, Handlers (Padrão CQRS) e consumidores do MassTransit para filas de mensagens.
3. **Camada de Domínio**: O núcleo do sistema. Contém Entidades, Enums, Value Objects e Eventos de Domínio. Não possui dependências externas.
4. **Camada de Infraestrutura**: Gerencia dependências externas. Contém o `DbContext` do EF Core, Repositórios, integrações com APIs REST de terceiros e configurações de Banco de Dados.

## Segurança e RBAC
Um Middleware estrito de Proteção de Rotas é implementado para extrair claims de usuários, validar tokens JWT e checar dinamicamente se as permissões de Módulo/Ação estão mapeadas para a Role no banco de dados, garantindo isolamento multilocatário (multi-tenant).

## Processamento Assíncrono
MassTransit é utilizado para prover filas de mensagens robustas, permitindo o processamento em background de tarefas como envio de e-mails, processos de faturamento automáticos ou webhooks.

## Testes
Cobertura abrangente através de testes de Unidade e de Integração usando `xUnit`. Utilização de bibliotecas de simulação (mocks) para isolar a lógica de domínio e assegurar confiabilidade.
