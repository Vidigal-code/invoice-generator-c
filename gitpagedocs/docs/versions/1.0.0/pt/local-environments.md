# Ambientes Locais e Compose

Para garantir um processo ágil no desenvolvimento local do Invoice Generator, usamos Docker.

## Rodando o Projeto
Execute `docker-compose up -d`. O comando fornece:
1. Instância do `SQL Server` para testes locais.
2. `Redis` para caching de sistema isolado.
3. `RabbitMQ` suportando o MassTransit em containers.
