# Primeiros Passos

Bem-vindo à documentação oficial e abrangente do **Invoice Generator C**.

Esta plataforma representa uma solução empresarial escalonável, implementada integralmente com fito de "Cloud-Native". Foi idealizada meticulosamente para calcular o custo e o fluxo temporal de dívidas em alta taxa de processamento, transacionar regras _multi-tenant_ exclusivas, e ditar a formalização inquebrável de novos acordos e a geração de boletos. Fundeada sob o Framework `.NET 8` emparelhado intimamente com `Angular 17`, une rigorosas diretrizes de negócios a uma grande imersão de UI/UX.

## 1. Pré-requisitos do Sistema

Para erguer esse ecossistema no desenvolvimento local, a virtualização em contêineres é estritamente exigida. Certifique-se de adotar integralmente as restrições de infraestrutura ditadas a seguir:

| Tecnologia | Versão Mínima | Objetivo Principal |
|------|-----------|-----------|
| **.NET SDK** | 8.0.x | Lida com os pacotes _NuGet_ de background e embasa os comandos do _EF Core Tool_. |
| **Node.js** | 18.17.x (LTS) | Ponto central de instalação de bibliotecas front-end do repositório. |
| **Angular CLI** | 17.0+ | Transpilação e monitoria com injetores modulares em local server. |
| **Docker Engine** | 24.x+ | Anfitrião que empacota e aloca simultâneamente todo leque multisserviço. |
| **Docker Compose** | V2 | Responsável pelo delineamento topológico contendo S3 e a base relacional. |

## 2. Iniciação do Ecossistema

Esta é uma arquitetura orientada e inteiramente distribuída, portando a enorme flexibilidade de ter seu setup construído e rodado em um formato semelhante ao da Produção apenas usando contêineres.

### Transferindo o Código (Clone)

```bash
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c
```

### Inicialização em Fast-Track (Via Docker)

O documento espalhado na raiz do repositório, `docker-compose.yml`, comporta todos os satélites de serviço dependentes:
- **api**: A interface Kestrel orientando a porta 8081.
- **frontend**: Angular estaticizado operando através do Proxy Nginx via _Bind_.
- **sqlserver**: Carga contendo instância do Microsoft SQL Edge nativo.
- **redis**: Subcamada de memória (Tranca Distribuídas / Locking).
- **rabbitmq**: Broker reativo fundamental para desamarração paralela nos retornos.
- **localstack**: Simulacro S3 garantindo ausência total de custos de hospedagem amazon.

```bash
# Constrói o fluxo descartando amarras no terminal de comando aberto
docker-compose up -d --build
```
> [!NOTE]
> O primeiro processo destas imagens pode demorar por longos minutos - há cargas colossais nas _LocalStacks_ e vastas dependências NuGet a serem catalogadas.

### Portas e Portais Identificados Localmente

Quando este orquestramento relatar finalização, eis os links nos quais a plataforma pode ser inteiramente controlada pelo ambiente corporativo:

| Endereço do Serviço | Caminho Local | Tipo e Foco de Ação |
|------|-----------|-------|
| Aplicação Frontend | `http://localhost:8081` | Portal Exclusivo dos Operadores |
| Documentação e APIs | `http://localhost:8081/swagger` | Testes para Analistas ou Devs |
| Console MassTransit (RMQ) | `http://localhost:15672` | Verificação de Filas e Subscrições |

## 3. Gestão Completa de Setup Avançada

Os caminhos normativos do projeto requerem que preenchas em seu `appsettings.Development.json` (ou `.env` de injeção Docker) mapeamentos e chaves altamente rigorosos para atalhar erros crônicos de inicialização indesejáveis.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=InvoiceGenerator;User Id=sa;Password=Sua_Senha_Forte_Aqui;TrustServerCertificate=True"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "Redis": {
    "ConnectionString": "localhost:6379"
  },
  "S3": {
    "Endpoint": "http://localhost:4566",
    "AccessKey": "test",
    "SecretKey": "test",
    "BucketName": "billets"
  },
  "Security": {
    "CorsOrigins": "http://localhost:8081,http://localhost:4200"
  },
  "AdminSettings": {
    "AdminEmail": "admin@system.local",
    "AdminPassword": "Admin@12345"
  }
}
```

## 4. Teardown Severo e Limpeza

Tempos vastos segurando seções executáveis de containers, mormente SQL e Logs de LocalStack injetados via RabbitMQ, tomam vasto gigantismo de armazenamento. Caso possuas urgência de purgar integralmente configurações falhas de forma bruta:

```bash
docker-compose down -v --rmi local
```
> [!WARNING]
> Empregue esta ferramenta com total cautela; a diretiva especial `-v` derreterá todo e qualquer dado guardado nos arquivos lógicos da database persistida (volumes docker). Eficiente caso o _Database Seeder_ corrompa no meio do caminho e precise de resete.
