# Guia Avançado de Deploy (Produção & CI/CD)

Saltar o abismo existencial conectório de um *Ecossistema Local em Docker* inteiramente simulado partindo para Produção corporativa escalonada é altamente resiliente graças à nossa adoção em separação de micro-camadas isoladas.

## 1. Mapeamento Topológico de Nuvens (Exemplo: AWS)

Se sua busca repousa na alta confiabilidade (Elasticiadade Horizontal Verdadeira), os recursos expostos no ambiente cru de controle traduzem-se para seus homólogos vitais e gerenciados:

| Contêiner de Treino | Equivalente Gerenciado Amazon (AWS) | Vantagem Escalar |
|------|-----------|-----------|
| **sqlserver** | AWS RDS (SQL Server Edition) | Backups granulares infalíveis. Escudos Multi-AZ mitigando catástrofes. |
| **redis** | AWS ElastiCache | Resposta absurdamente iminente fortalecendo o *Distributed Locking* crucial da formalização. |
| **rabbitmq** | Amazon MQ (RabbitMQ broker) | Remove toda necessidade manual de purgar _dead letters_ corrompidas reativando nós apagados manualmente. |
| **api** | ECS Fargate ou Pods em EKS | Contêiner auto-escalável explodindo horizontalmente durante épocas cruéis de fechamento contábil mensal de cobranças. |
| **frontend** | Bucket S3 Estático + CND CloudFront | Despacha bytes nativos por cache distribuído perante CDNs globais sem gargalos em servidor físico backend. |
| **localstack** | AWS S3 Bucket Nativo | Cofre fortificado trancafiando e presignando conexões do arquivo PDF nativamente. |

## 2. Abstração de Automação (Ex. GitHub Actions)

Uma rampa ideal para tracionar fluidez num `deploy.yml`. Amassa o Angular nativamente despachando e construindo ao mesmo tempo verificações puras das malhas no backend originando as instâncias lean.

```yaml
name: Orquestração de Build Mestra
on:
  push:
    branches: [ "main" ]

jobs:
  build_frontend:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Prepara Node
      uses: actions/setup-node@v3
      with:
        node-version: '18.x'
    - name: Compressão Pura Estática
      run: |
        npm install
        npm run build --configuration=production

  build_backend:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Ambiente de Montagem .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Peneira de Testes Unitários Segregados
      run: dotnet test
    - name: Montagem de Container Lean Cíclico
      run: docker build -t igc-api ./backend
```

## 3. Armadilhas do Entity Framework Migration

Lembrete abslutamente inegociável: jamais dispare `context.Database.Migrate()` soltos dentro dos laços autônomos `Program.cs` se vais possuir mais de 1 Pod Kubernetes escalonado ativamente subindo simultaneamente — a catástrofe recai com falhas mortais em concorrência tentando trancar a tabela de logs interna `__EFMigrationsHistory` concomitantemente na fundação estrutural corrompendo parciais esquemas.
- Separe em _Stages de Pipeline Dedicados_ rodando ef core autônomo num container descartável rodado singularmente sobre o contexto restrito ou delegue uma task única e cega pré-deploys dos conteineres gerais.
