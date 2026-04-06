# Referência da API

O Backend expõe uma API RESTful completa e documentada simultaneamente via Swagger e ReDoc com suporte bilingue (Inglês e Português).

## Endpoints Principais

### Autenticação (`/api/Auth`)
- `POST /login`: Recebe credenciais e devolve Cookies HttpOnly seguros e estritos contendo o JWT de autorização. Limite de requisições rigoroso (Rate Limiting).
- `POST /register`: Cria usuários seguindo o `strongPasswordValidator`.

### Admin Panel (`/api/AdminPanel`)
Restritos a Administradores (verificados no `RouteProtectionMiddleware`).
- `GET /logs`: Retorna logs de auditoria detalhados emitidos pelo Audit Service.
- `GET /contracts`: Lista completas as interações dos clientes e históricos no sistema.

### Pagamentos e Acordos (`/api/Agreements`)
- `POST /formalize`: Gatilho principal para gerar boleto ou iniciar pagamentos via PIX. Conta com proteção *Distributed Locking* baseada em Redis.
- `GET /billet/{id}`: Rota que devolve o IFRAME source e os headers de proxy corretos para exibição sem base64 leaks.

## Rate Limiting e Payload Restrictions
O sistema garante escalabilidade segurando payloads imensos. Requisições maiores que o tamanho permitido (geralmente poucos megabytes) e fluxos massivos de IPs não confiáveis levam a um imediato `429 Too Many Requests`.
