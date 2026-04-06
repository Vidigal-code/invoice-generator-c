# CI/CD e Implantação

O ecossistema de Integração Contínua (CI) e Entrega Contínua (CD) do Invoice Generator possui automação robusta.

## Fluxos de Trabalho do GitHub Actions
Nossos fluxos garantem segurança do commit ao deploy final:
- **Deploy do GitPageDocs**: Nós usamos workflows do GitHub (`gitpagedocs-pages.yml`) que automatizam o processo de renderização e build dos markdowns para páginas HTML publicadas perfeitamente e de forma gratuita usando GitHub Pages.

## Containerização
- **Docker**: Tanto o Frontend quanto o Backend usam camadas "multi-stage" em seus `Dockerfile`s, produzindo contêineres finais extremamente leves.
- **Docker Compose**: Oferece um meio de padronizar a instalação local e os testes com containers (Postgres, Redis, e Brokers).

## Proxy Reverso
- **NGINX**: Usado em ambiente de produção para agilizar requisições estáticas do Angular e fazer proxy reverso (Reverse Proxy) de chamadas de rede confiaveis aos containeres internos de .NET Core, livrando o projeto da complexidade das regras de CORS.
