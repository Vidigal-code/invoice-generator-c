# Camada de Aplicação e CQRS

A camada de **Aplicação** define os fluxos de negócios e a orquestração do sistema, servindo de ponte entre as requisições API/UI e as regras de Domínio.

## Padrão CQRS
Seguimos rigorosamente o padrão CQRS (Command Query Responsibility Segregation):
- **Commands (Comandos)**: Encapsulam qualquer operação que altere o estado da aplicação. Executado por Handlers e MediatR.
- **Queries (Consultas)**: Operações de apenas leitura que retornam payloads otimizados para view, sem disparar efeitos colaterais.

## Validações
- **FluentValidation**: Implementado dentro dos pipelines da camada de Aplicação. Quando um Comando trafega através do MediatR, os comportamentos de validação asseguram que requisições com dados em formato inválido retornem um erro `BadRequest` sem afetar ou acionar a camada de domínio.

## Consumidores de Mensagens
- **Consumidores MassTransit**: Classes dedicadas (`IConsumer<T>`) nesta camada lidam com toda a rotina de dezenas funções assíncronas do sistema, processando e reagindo a eventos de domínio – como o sucesso na emissão de faturas ou a trilha de auditoria.
