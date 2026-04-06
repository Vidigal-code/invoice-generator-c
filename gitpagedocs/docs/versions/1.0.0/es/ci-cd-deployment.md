# CI/CD e Implementación (Deployment)

La plataforma Invoice Generator tiene un entorno robusto de Integración y Entrega Continua automatizada.

## Flujos de Trabajo de GitHub Actions
Manejamos el ciclo de vida desde el repositorio hasta los servidores:
- **GitPageDocs CI**: Contamos con rutinas (.yml) dedicadas a convertir nuestra base de documentación multilingüe elaborada en markdown nativamente a GitHub Pages HTML sin intervención humana.

## Contenedores
- **Docker**: Utilizamos empaquetado multi-stage (Dockerfiles) en nuestros proyectos de backend (.NET) y frontend (Angular) logrando reducir enormemente el peso de las imágenes finales de lanzamiento.
- **Docker Compose**: Nos ayuda como estándar del entorno desarrollador, replicando una red completa con PostgreSQL, Redis y otros requerimientos locales fácilmente.

## Proxy y NGINX
- **NGINX**: Ocupa el rol de servir el marco estático compilado y proporcionar un Proxy Inverso rápido dirigiendo el tráfico a las APIs internas en .NET. Esto no solo mejora la seguridad, sino que también evita complejas configuraciones de origen cruzado (CORS).
