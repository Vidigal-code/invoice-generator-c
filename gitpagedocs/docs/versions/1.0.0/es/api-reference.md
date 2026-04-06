# Detalles Estrictos OpenAPI / Swagger

La **Invoice Generator C** estipula y protege los pasajes en puro acatamiento estructural de protocolos RESTful.

## 1. Estrato Inicial Base Requerido (`/api/Auth`)

La inmersión del operador requiere saludo validado propiciando empaquetaduras inalterables de red: `HttpOnly Cookies`.

### `POST /api/Auth/login`
El puente vital para validación analítica temporal. Limitado de forma agresiva mediante contadores defensivos Rate-Limiter atajando dictados y abusos malintencionados con múltiples pruebas (Dictionary Attacks).

**Estructura del Intercambio (JSON Payload):**
```json
{
  "email": "admin@system.local",
  "password": "Admin@12345_Str0ng"
}
```

**Retribución Operativa (200 OK + Intercepción Nativa `Set-Cookie` Headers):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "accessToken": "eyJhbG...", // Absorbido sutilmente mediante navegadores de confianza en segundo plano
    "userName": "Asignación Plataforma",
    "roles": ["Admin", "Super"]
  }
}
```

## 2. Visores Panel Historiales Administradores (`/api/AdminPanel`)

Obstruido operativamente para tránsitos simples. Gobierna lógicas ciegas blindadas provenientes del contorno `.NET RouteProtectionMiddleware`.

### `GET /api/AdminPanel/logs?page=1&limit=25`
Indaga huellas marcadas incansablemente por auditores in-house con orígenes identificativos IP pseudo-ofuscados.

**Demostración Restitución Limitada (200 OK):**
```json
{
  "totalItems": 1599,
  "currentPage": 1,
  "items": [
    {
      "id": "e434cd...",
      "timestamp": "2024-03-01T15:00:23Z",
      "level": "Warning",
      "message": "Infracción transaccional atajada por Tranca de Memoria.",
      "maskedIp": "192.168.***.***",
      "userId": "90fe-421..."
    }
  ]
}
```

## 3. Concreción Digerida Hacia Deudas Vivas (`/api/Agreements`)

Delegador orquestal que mueve engranajes ocultos paralelos amarrados tras muros de Memoria Abierta Rápida (Redis) disparando estampadoras en C# (QuestPDF).

### `POST /api/Agreements/formalize`
Desacoplador robusto solicitando en primera orden total bloqueador limitante contra peticiones clónicas.

**Inyección Acordada:**
```json
{
  "contractId": "bca8b789-54d1...",
  "paymentMethod": "PIX_BILLET",
  "agreedTotalValue": 1055.99
}
```

**Rechinado De Inmunidad Simultáneo (409 Conflict):**
```json
{
  "success": false,
  "errors": {
    "message": "Fijación estricta correntada actualmente impidiendo solapamientos. Reintente ulteriormente."
  }
}
```

## 4. Retorno Efímero Y Resguardado

### `GET /api/Agreements/billet/{id}`
Saca a flote contraseñas temporales al vuelo de lectura efímera (`Pre-Signed AWS Mock Links`) acoplándose inteligentemente contra subcajas web puras `Iframes`. Rehúye despóticamente colapsos lentos sufridos enviando cargas titánicas crudas por secuencias binarias directas `Base64` al front.
