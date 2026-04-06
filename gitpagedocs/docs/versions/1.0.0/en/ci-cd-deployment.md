# CI/CD and Deployment Pipelines

Continuous Integration and Continuous Deployment for the Invoice Generator are highly automated.

## GitHub Actions Workflows
Our workflows guarantee a secure lifecycle from commit to deployment:
- **GitPageDocs CI**: We maintain dedicated `.github/workflows` (such as `gitpagedocs-pages.yml`) which statically generate the markdown portals across multiple languages directly into GitHub Pages entirely without server overhead.

## Containerization
- **Docker**: For primary environments, our Backend and Frontend build rules rely on multi-stage `Dockerfile` layers minimizing the final image sizes and isolating dependencies.
- **Docker Compose**: Ensures developer environments maintain consistency in dependency instantiation (like Postgres, Redis, and message broker queues).

## Reverse Proxy and Load Balancing
- **NGINX**: Utilized not only for serving the angular front end build bundle but executing fast reverse proxies routing frontend traffic consistently to internal .NET microservice APIs, drastically cutting down on CORS configuration complications.
