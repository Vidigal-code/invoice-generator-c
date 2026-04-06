# Advanced Deployment Guide (CI/CD)

Transitioning from a mock-heavy isolated Docker environment into Production is highly supported and architecturally straight.

## 1. Cloud Ecosystem Targets (AWS Example)

For true horizontal elasticity, deploying into **Amazon Web Services (AWS)** relies heavily mapping your local elements into robust managed instances.

| Local Container | AWS Managed Service Equivalent | Purpose in Production |
|------|-----------|-----------|
| **sqlserver** | AWS RDS (SQL Server) | Automated backups, multi-AZ reliability. |
| **redis** | AWS ElastiCache | Absolute extreme IO metrics for safe distributed locks. |
| **rabbitmq** | Amazon MQ (RabbitMQ) | Eradicates custom broker downtime management. |
| **api** | ECS Fargate or EKS Pod | Scales automatically adjusting compute capacities matching HTTP thresholds. |
| **frontend** | S3 Static + CloudFront | Delivers Angular files blazingly fast around the globe. |
| **localstack** | AWS S3 Bucket | Secured document attachments naturally natively stored protecting generated Boletos. |

## 2. GitHub Actions CI/CD Pipeline Mockup

An exemplary `deploy.yml` pipeline pushes the Angular builder heavily distributing into Cloudfront domains whilst running dotNet commands generating lean images.

```yaml
name: Global Build Pipeline
on:
  push:
    branches: [ "main" ]

jobs:
  build_frontend:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Node Prep
      uses: actions/setup-node@v3
      with:
        node-version: '18.x'
    - name: Static Gen
      run: |
        npm install
        npm run build --configuration=production

  build_backend:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: .NET Sandbox
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    - name: Compile Tests
      run: dotnet test
    - name: Push Container
      run: docker build -t igc-api ./backend
```

## 3. Entity Framework Data Handlers

Never run `context.Database.Migrate()` autonomously bundled alongside several parallel scaled backend API Instances on boot — this invokes huge lock catastrophes within RDS bounds resulting into partial migrations applying destructively.
- Segregate database seeding onto a dedicated Pipeline Stage (Ex: *Migration Worker* container executed uniquely, running EF Core CLI directly on top of the fresh artifact instance pointing to your safe environment string variable).
