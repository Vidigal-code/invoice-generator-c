# Guía de Despliegue

**Invoice Generator C** fue diseñado desde su concepción para estar altamente contenizado. De esta manera, habilitamos un paso natural con el menor número de fallas desde instancias de experimentación local hacia un entorno Cloud Corporativo.

## 1. Entornos y Configuraciones Variables

Antes de la puesta en escena para Producción Estricta, asegúrese fuertemente de proveer o inyectar las variables apropiadas en las rutas designadas de los micro-servicios:
- `ConnectionStrings__DefaultConnection`: Refleja el Punto de Conexión de su base SQL definitiva (Production).
- `RabbitMQ__Host`, `RabbitMQ__Username`, `RabbitMQ__Password`: Credenciales protegidas exclusivas del ambiente MQ.
- `Redis__ConnectionString`: Conexión directa a servicios Redis de alto desempeño y/o clústeres.

## 2. Abordaje en Docker Compose (Pruebas y Staging)

Fácilmente identificable en la raíz matriz inicial se ubica un consolidado `docker-compose.yml`. A modo Staging o Previo, puede reutilizar este molde general:

```bash
docker-compose -f docker-compose.yml up -d
```
El mandato enciende las cascadas de interservicios de forma encadenada (Backend, Frontend atado a Nginx, Simuladores S3 Localstack, Bases puras de datos...).

## 3. Despliegue a Producción Verdadera (Modelos CI/CD)

En un ambiente catalogado de grado **Producción (Production-Grade)** (AWS ECS, Kubernetes, Interacciones Azure Apps), debe dividirse notoriamente su modelo de ensamble unitario en contenedores dedicados:
- **Backend**: Generación absoluta utilizando solo en las directivas que dictan su `Dockerfile` matriz.
- **Frontend (Angular)**:
    ```bash
    npm run build --configuration=production
    ```
    Aquellos artefactos terminados tras su compresión en la ruta (`dist/`) se empaquetan en solitario junto con la versión web Nginx que provee el código en (`docker/nginx/default.conf`). La ruta aloja de forma paralela la comunicación estática abierta (`Port 80`) y encauza correctamente peticiones provenientes del `/api` al backend sin interferencias.

## 4. Migraciones de Datos (Entity Framework)

Los motores de Entity Framework en la familia de tecnología DotNet actual no procesarán en automático cargas de migración profunda dentro de su contenedor general sin orden expresa ubicada dentro del modelo `Program.cs`. Confirme siempre que el contenedor instanciador dotNet puede visibilizar transparentemente el servicio del puerto SQL teniendo permisos plenos al momento de mandar secuencias `context.Database.Migrate()`. Por otra parte se encuentra a disposición correr directamente semillas primigenias como las ubicadas estructuralmente bajo denominación `init.sql`.
