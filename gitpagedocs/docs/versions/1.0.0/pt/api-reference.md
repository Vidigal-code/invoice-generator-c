# Especificações Rigorosas da API REST

A **Invoice Generator C** estipula rotas em pleno acordo às diretrizes severas de RESTful.

## 1. Camada de Autenticação Central (`/api/Auth`)

A imersão primária requisita aperto de mão (handshake) retornando _encapsuladores HttpOnly Cookies_.

### `POST /api/Auth/login`
Principal portão de validação de sessões. É restrito agressivamente aos filtros *Rate-Limiter* prevenindo ataques de dicionários brutos analisados via IP.

**Estrutura de Entrada (JSON Payload):**
```json
{
  "email": "admin@system.local",
  "password": "Admin@12345_Str0ng"
}
```

**Retorno Base (200 OK + Extratos de `Set-Cookie` Headers):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "accessToken": "eyJhbG...", // Evaporado no front e armazenado seguidamente sob Cookies do Browser.
    "userName": "Administrador Plataforma",
    "roles": ["Admin", "Super"]
  }
}
```

## 2. Gestão Administrativa Logs/Auditoria (`/api/AdminPanel`)

Bloqueio terminal operando dentro da lógica cega do pipeline do `.NET RouteProtectionMiddleware`. Apenas níveis super-usuários transitam.

### `GET /api/AdminPanel/logs?page=1&limit=25`
Varre detalhadamente trilhas de auditoria contendo marcas inalteráveis de IPs formatados assimetricamente de operários sistêmicos.

**Extrato de Matriz Respondida (200 OK):**
```json
{
  "totalItems": 1599,
  "currentPage": 1,
  "items": [
    {
      "id": "e434cd...",
      "timestamp": "2024-03-01T15:00:23Z",
      "level": "Warning",
      "message": "Operador tentou violar travamento.",
      "maskedIp": "192.168.***.***",
      "userId": "90fe-421..."
    }
  ]
}
```

## 3. Formalização Operatória de Dívidas (`/api/Agreements`)

Conjuntos orquestrando pesadas ações que amarram motores distribuídos em Memória RAM (Redis) finalizando na prensa de documentos QuestPDF.

### `POST /api/Agreements/formalize`
Requisições pesadas que englobam exclusividade matemática e retração para finalização de acordos.

**Demanda Estrutural:**
```json
{
  "contractId": "bca8b789-54d1...",
  "paymentMethod": "PIX_BILLET",
  "agreedTotalValue": 1055.99
}
```

**Reação a Gatilho Simultaneou Falso (409 Conflict):**
```json
{
  "success": false,
  "errors": {
    "message": "Formalização correndo na retaguarda. Acordo sob o mesmo identificador encontra-se devidamente travado!"
  }
}
```

## 4. Retorno Desacoplado Visual (Boleteria)

### `GET /api/Agreements/billet/{id}`
Fornece chaves curtas e pontuais para acesso temporário (Pre-Signed AWS Mocks no S3) re-inseridas perante `Iframes` fechados, evadindo com mestria as gigantes lentidões ao se enviar dados textuais puros carregados engolindo bytes da camada backender sob formatos lentos de Array ou Base64.
