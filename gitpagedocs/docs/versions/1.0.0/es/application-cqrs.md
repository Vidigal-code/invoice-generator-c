# Capa de Aplicación y CQRS

La capa de **Aplicación** define el flujo de trabajo del negocio y la orquestación, sirviendo como puente entre las peticiones API/UI y las restricciones de Dominio.

## Patrón CQRS
Se sigue rigurosamente el paradigma CQRS:
- **Commands (Comandos)**: Encapsulan todas las operaciones que modifican el estado de la aplicación. Gestionados eficientemente vía MediatR.
- **Queries (Consultas)**: Operaciones de solo lectura que entregan conjuntos de datos optimizados, sin desencadenar efectos secundarios.

## Validaciones
- **FluentValidation**: Implementado a través de "Pipeline Behaviors" en MediatR. Impide que las peticiones que no cumplan los criterios formales superen la entrada a la capa de Aplicación, devolviendo rápidamente una excepción o mensaje `BadRequest`.

## Consumidores de Mensajería
- **MassTransit Consumers**: Clases receptoras (`IConsumer<T>`) procesan tareas asíncronas masivas en este nivel, atendiendo eventos lógicos como notificaciones de facturas terminadas o registros en las trazas de auditoría.
