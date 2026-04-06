# Guia de Deploy

O **Invoice Generator C** é conteinerizado, permitindo que a transição de um ambiente local para Produção (Cloud) seja natural e menos propensa a erros de ambiente limitados.

## 1. Ambientes e Variáveis

Antes do Build de Produção, é essencial ter certeza de injetar as variáveis corretas para as conexões dos containers:
- `ConnectionStrings__DefaultConnection`: Endereço final do seu Banco SQL de Produção.
- `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`: Credenciais corretas do ambiente RabbitMQ de Produção.
- `Redis__ConnectionString`: Conexão ao serviço de Redis de alta performance ou Cluster.

## 2. Docker Compose (Local e Staging)

Na raiz do projeto, temos o arquivo `docker-compose.yml`. Em cenários de Staging, você pode reutilizá-lo:

```bash
docker-compose -f docker-compose.yml up -d
```
Ele subirá toda a cascata (Backend, Nginx Frontend, Localstack para Mocks, e Bases de Dados).

## 3. Deploy de Produção (CI/CD Recomendado)

Para ambientes **Production-Grade** (AWS ECS, Kubernetes, Azure Container Apps), a compilação costuma ser desmembrada:
- **Backend:** Geração da imagem via `Dockerfile`.
- **Frontend (Angular):**
    ```bash
    npm run build --configuration=production
    ```
    Os artefatos produzidos (a pasta `dist/`) são empacotados pelo próprio Nginx provido no repositório (`docker/nginx/default.conf`) que atua distribuindo os estáticos em `Port 80`, repassando `/api` ao upstream correto.

## 4. Migrations no Entity Framework

O Entity Framework core não roda as migrations automaticamente em containers a menos que configurado explicitamente no Program.cs. Assegure-se de que o container do C# consiga atingir o Banco de Dados e possua privilégios se estiver aplicando migrations automáticas `context.Database.Migrate()` em ambiente. Outra abordagem é injetar um inicializador SQL (presente como `init.sql`).
