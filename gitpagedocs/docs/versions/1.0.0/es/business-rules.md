# Reglas de Negocio

El sistema ha sido diseñado en torno a rigurosos dominios corporativos y flujos de formalización seguros. A continuación, se detallan las reglas del negocio bajo la perspectiva del sistema **Invoice Generator C**.

## 1. Gestión y Cálculo de Deudas

Cada vez que el usuario visualiza una deuda (Contracts), el cálculo exacto del monto pasa por una estrategia dedicada denominada `InvoiceGeneratorCDebtCalculationStrategy`.
- Las deudas pueden contener recargos, intereses y multas calculados al momento (on-the-fly) o procesados previamente.
- Los Contratos (Contracts) tienen una relación de 1 a N con las sub-deudas (portafolios/facturas).

## 2. Acuerdos y Formalización

La formalización de un acuerdo es la acción primordial del sistema.
- Al solicitar un acuerdo, el Backend invoca el `CreateAgreementCommandHandler`.
- **Prevención de Condiciones de Carrera:** Antes de iniciar cualquier registro, el sistema asegura el `RedisDistributedLock` sobre la deuda/contrato a usar. Esto garantiza que peticiones simultáneas (ej. doble clic en el frontend) no resulten en dos boletos diferentes asignados a la misma deuda.
- De obtenerse el Lock, la deuda queda formalizada, y un **Boleto** en formato PDF es emitido mediante la librería `QuestPDF`.

## 3. Emisión de Boletos (Billets)

La visualización del Boleto es altamente segura por retención:
- El boleto (PDF) no se devuelve a través de Base64 sin seguridad. Se cifra y se guarda directamente en S3 LocalStack.
- Se devuelve un enlace temporal y seguro manejado por el sandbox del IFrame en el frontend (`BilletViewerComponent`).

## 4. RBAC y Protecciones de Seguridad

La estructura de Tenant y el Control de Acceso Basado en Roles (RBAC) están fuertemente arraigados en la aplicación.
- La exclusividad de los permisos se valida en el middleware `RouteProtectionMiddleware.cs`.
- Todo evento sensible, error o alteración se registra en el **Audit Service**, en el que las direcciones IP son procesadas con un formato de seudoanonimización (mascaramiento IP AES-256).
