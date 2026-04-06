# Application Layer & CQRS

The **Application** layer defines the business workflow and orchestration, coordinating between UI/API requests and Domain constraints.

## CQRS Pattern
We rigorously follow Command Query Responsibility Segregation (CQRS):
- **Commands**: Encapsulate any operations that mutate application state. Supported strongly by MediatR handlers.
- **Queries**: Read-only operations retrieving customized data payloads, without triggering Side-Effects.

## Validations
- **FluentValidation**: integrated deeply within the Application layer pipelines. When a Command flows through MediatR, FluentValidation pipeline behaviors guarantee that invalid requests instantly yield `BadRequest` responses avoiding processing corrupted state.

## Message Consumers
- **MassTransit Constraints**: Dedicated consumer instances (`IConsumer<T>`) process asynchronous messaging routines natively here, reacting to domain events such as invoice completions, and relay audits to reporting endpoints.
