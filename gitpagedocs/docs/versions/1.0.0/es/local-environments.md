# Entornos Locales y Compose

Para asegurar la fluidez implementando el generador de facturas, nos basamos en Docker.

## Despliegue Local
Inicia todo ejecutando `docker-compose up -d`. Instanciarás:
1. `SQL Server` de forma nativa sin setups complejos.
2. `Redis` para simular caching realista.
3. `RabbitMQ` que interconecta todos los consumos asincrónicos.
