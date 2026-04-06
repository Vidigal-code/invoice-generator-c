# Primeros Pasos

Bienvenido a la documentación oficial y altamente detallada del portal **Invoice Generator C**.

La plataforma constituye una propuesta empresarial nativa de la Nube conformada integralmente para controlar altos volúmenes de cálculo de deudas y su formalización consecuente en arreglos, apoyada desde el core `.NET 8` mediante una fachada elaborada puramente utilizando el entorno de `Angular 17`.

## 1. Requisitos Indispensables

Toda la interconexión local del sistema ha sido amparada usando técnicas pesadas en Virtualización vía Docker Engine. Por lo tanto, garantice estar blindado con el equipamiento aquí resaltado:

| Herramienta | Versión Sugerida | Proósito Dentro Del Sistema |
|------|-----------|-----------|
| **.NET SDK** | 8.0.x | Construcción primordial del backend e interacción del CLI general de EF Core. |
| **Node.js** | 18.17.x (LTS) | Pilar elemental para descargar repositorios de base al servidor cliente. |
| **Angular CLI** | 17.0+ | Puntos de vigilancia interactiva en rearmado y generación SPA local. |
| **Docker Engine** | 24.x+ | Entidad encargada de orquestar toda base perimetral del ecosistema. |
| **Docker Compose** | V2 | Vinculador oficial para dictaminar el árbol de procesos dependientes. |

## 2. Puesta En Operación del Entorno

Todo rincón del código ha mutado a estar sumamente seccionado y distribuido en subestaciones, esto trae el brillante provecho en el que puedes reproducir un escenario de Nivel de Producción solo levantando el _Compose_.

### Clonando la Arquitectura Base

```bash
git clone https://github.com/vidigal-code/invoice-generator-c.git
cd invoice-generator-c
```

### Inicialización Fast-Track (Docker)

El archivo `docker-compose.yml`, el cual vigila sobre el inicio del repositorio, acopla los servicios cruciales enumerados aquí:
- **api**: A la cabeza guiando el tráfico en el puente expuesto `8081` a través de _Kestrel_.
- **frontend**: Capa de aplicación uniplágina unida en proxy pasante vía *Nginx*.
- **sqlserver**: Relacional que enfrasca una versión robusta local Edge SQL.
- **redis**: Esencial como una red blindada de colisiones para las Formalizaciones de Pagos simultáneos y _Locks_.
- **rabbitmq**: El Broker y cerebro delegador que despacha y absorbe tareas al _background_.
- **localstack**: Simulador nativo en tiempo real que oculta nuestra interdependencia AWS garantizando retención a través de depósitos en S3.

```bash
# Lanzar un set sin amarrarse del flujo interactivo
docker-compose up -d --build
```
> [!NOTE]
> Su compilación número cero podrá costarle múltiples minutos tras la recuperación totalitaria de librerías del backend (NuGet), front-end compresión de binarios (NPM) y absorciones demoradas del contenedor de S3.

### Mapeo de Puntos de Acceso Rápido

Luego de confirmada la construcción en el contorno dictaminado por los loggers, aquí están sus atajos hacia el monitoreo de componentes:

| Entidad Virtual | Atajo Recomendado | Meta Estricta |
|------|-----------|-------|
| Interfaz Global Angular | `http://localhost:8081` | Tablero (Dashboard) Frontal Operacional |
| Referencias OpenAPI Swagger | `http://localhost:8081/swagger` | Ensayos Controlados (Endpoints y Headers) |
| Console MassTransit RMQ | `http://localhost:15672` | Monitoreo Activo sobre Las Colas en Uso |

## 3. Parametrización y Variables Críticas

Deberá poseer en su documento guía principal (en `appsettings.Development.json` o al configurar variables sobre el marco en `.env`) llaves estrictamente forjadas que evitan el deceso crónico de acceso al inicializar los registros.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=InvoiceGenerator;User Id=sa;Password=Clave_Segura#1A;TrustServerCertificate=True"
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

## 4. Teardown Controlado (Limpieza Destructiva)

Mantener vastos archivos y registros por innumerables horas, primordialmente los de S3 o rastros póstumos de RabbitMQ, suele derivar en una gran erosión al almacenamiento temporal de la PC local. Ante cualquier deseo de borrar lo generado (reseteo abrupto):

```bash
docker-compose down -v --rmi local
```
> [!WARNING]
> Aplíquese tan solo habiéndose certificado en cuenta previa que todo bloque persistente (Docker volumes) en el interior virtual de bases de datos será devastado. Esta directiva soluciona problemas extraños que aparecen sobre el _Seeder_, pero fulminará instantáneamente cuentas añadidas.
