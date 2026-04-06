# Rigorous API Specifications

The **Invoice Generator C** strictly defines endpoints adhering to true REST standards.

## 1. Authentication Layer (`/api/Auth`)

To interact deeply with protected routes, developers must initiate session handshakes generating `HttpOnly Cookies`.

### `POST /api/Auth/login`
The primary authentication gateway. It is aggressively rate-limited tracking incoming IP blocks preventing bruteforce assaults.

**Request Payload:**
```json
{
  "email": "admin@system.local",
  "password": "Admin@12345_Str0ng"
}
```

**Response (200 OK + `Set-Cookie` Headers):**
```json
{
  "success": true,
  "statusCode": 200,
  "data": {
    "accessToken": "eyJhbG...", // Ignored by Angular, utilized solely by native clients
    "userName": "System Administrator",
    "roles": ["Admin", "Super"]
  }
}
```
*Note: Browsers process the raw JWT cookie implicitly securely holding it aside.*

## 2. Admin Logs Panel (`/api/AdminPanel`)

Authorized via extreme validation inside `.NET RouteProtectionMiddleware` requiring `"Admin"` roles exclusively.

### `GET /api/AdminPanel/logs?page=1&limit=25`
Lists actions systematically generated via system-auditing loops covering sensitive modifications.

**Response Schema Extract (200 OK):**
```json
{
  "totalItems": 1599,
  "currentPage": 1,
  "items": [
    {
      "id": "e434cd...",
      "timestamp": "2024-03-01T15:00:23Z",
      "level": "Warning",
      "message": "User failed locking request.",
      "maskedIp": "192.168.XXX.XXX",
      "userId": "90fe-421..."
    }
  ]
}
```

## 3. Operations & Debt Formalizations (`/api/Agreements`)

These routes manipulate heavy loads executing multi-tier logic (Distributed Locking + PDF generations via `QuestPDF`).

### `POST /api/Agreements/formalize`
Initiates lock-fetching to claim debt resolution.

**Request Payload:**
```json
{
  "contractId": "bca8b789-54d1...",
  "paymentMethod": "PIX_BILLET",
  "agreedTotalValue": 1055.99
}
```

**Conflict Handling (409 Conflict):**
```json
{
  "success": false,
  "errors": {
    "message": "A formalization lock is already under active occupancy for this localized Debt."
  }
}
```

## 4. Billet Visualization Extractor

### `GET /api/Agreements/billet/{id}`
Generates direct proxy-enabled sandboxed IFRAME sources parsing strictly pre-signed URLs resolving towards local AWS Mocks (LocalStack). Escapes Base64 rendering payload inflations minimizing backend string allocations completely.
