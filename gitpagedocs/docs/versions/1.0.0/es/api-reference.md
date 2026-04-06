# Referencia de la API

El Backend expone una API RESTful exhaustiva, documentada a la vez mediante Swagger y ReDoc, que cuenta con soporte bilingüe (inglés y portugués).

## Endpoints Principales

### Autenticación (`/api/Auth`)
- `POST /login`: Procesa credenciales y retorna Cookies HttpOnly en su versión más estricta con el respectivo JWT. Está sujeto a regulaciones fuertes de límites de petición (Rate Limiting).
- `POST /register`: Registra cuentas respetando las reglas delineadas bajo el protocolo `strongPasswordValidator`.

### Panel Admin (`/api/AdminPanel`)
Endpoints con permisos limitados y de corte exclusivo para Administradores (verificados de primera mano en `RouteProtectionMiddleware`).
- `GET /logs`: Retorna los completos registros de auditoría que inician su rastro dentro del Audit Service.
- `GET /contracts`: Lista completa de interacciones con el portafolio y los historiales de los clientes en contexto general.

### Pagos y Acuerdos (`/api/Agreements`)
- `POST /formalize`: Principal detonador que lanza un boleto inicial o una operación con PIX. Goza de un blindaje fundamentado en Redis llamado *Distributed Locking*.
- `GET /billet/{id}`: Devuelve la fuente de visualización y headers de proxy pertinentes para que el IFAme renderice el boleto sin recaer a vulnerabilidades de Base64.

## Rate Limiting y Restricciones de Payload
El sistema resguarda su eficiencia limitando los envíos o descargas sobredimensionadas. Las peticiones considerables (generalmente superiores a unos cuantos megabytes) y ataques abrumadores reciben la devolución automática del estado `429 Too Many Requests`.
