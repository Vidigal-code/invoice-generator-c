# Primeiros Passos

Bem-vindo à documentação oficial do **Invoice Generator C**.

Esta plataforma corporativa é um sistema completo de cálculo de dívidas, formalização de acordos e gestão com recursos avançados de auditoria e segurança.

## Pré-requisitos

Para executar o projeto, você precisará de:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/) e Angular CLI 17+
- [Docker e Docker Compose](https://www.docker.com/)

## Instalação e Execução (Docker)

A forma mais fácil de rodar o projeto localmente com todos os seus serviços dependentes (SQL Server, Redis, RabbitMQ, LocalStack S3) é utilizando o Docker Compose.

```bash
# Clone o repositório
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c

# Suba a infraestrutura completa via Docker
docker-compose up -d
```

Acesse os serviços:
- **Painel Frontend**: http://localhost:8081
- **API Swagger/ReDoc**: http://localhost:8081/swagger
- **RabbitMQ Management**: http://localhost:15672

## Variáveis de Ambiente Essenciais

As seguintes variáveis configuram o ambiente em `appsettings.json` ou variáveis do Docker:
- `ConnectionStrings:DefaultConnection`: Conexão ao SQL Server.
- `Security:CorsOrigins`: Origens frontend permitidas para evitar bloqueio CORS.
- `AdminSettings:AdminEmail` / `AdminSettings:AdminPassword`: Credenciais do primeiro administrador no Seed do sistema.

## Scripts npm e Comandos .NET

**Backend:**
```bash
cd backend
dotnet restore
dotnet build
dotnet run --environment Development
```

**Frontend:**
```bash
cd frontend
npm install
npm start
```
