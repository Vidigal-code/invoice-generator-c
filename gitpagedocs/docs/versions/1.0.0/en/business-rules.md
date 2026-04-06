# Business Rules

The system has been designed around rigorous enterprise domains and safe formalization flows. Here we detail the business rules from the perspective of the **Invoice Generator C** system.

## 1. Debt Management and Calculation

Every time the user visualizes a debt (Contracts), the exact amount is calculated through a dedicated strategy called `InvoiceGeneratorCDebtCalculationStrategy`.
- Debts may contain extra charges, interest, and fines calculated either on-the-fly or pre-processed.
- Contracts have a 1-to-N relationship with sub-debts (portfolios/invoices).

## 2. Agreements and Formalization

Formalizing an agreement is the system's core action.
- When requesting an agreement, the Backend hits the `CreateAgreementCommandHandler`.
- **Race Condition Prevention:** Before initializing any creation, the system triggers the `RedisDistributedLock` for the targeted debt/contract. This ensures that simultaneous requests (e.g., frontend double-clicks) will not result in two different billets for the same debt.
- If the Lock is acquired successfully, the debt is formalized, and a **Billet (Boleto)** in PDF format is emitted using the `QuestPDF` library.

## 3. Billet Emission (Billets)

Visualizing the Billet encompasses Security and Retention:
- The billet (PDF) is not insecurely returned through Base64. It is instead encrypted and saved directly into S3 LocalStack.
- A secure temporary link is returned in conjunction with an IFrame sandbox (`BilletViewerComponent`) in the frontend.

## 4. RBAC and Security Protections

Tenant structure and Role-Based Access Control (RBAC) are deeply rooted in the application.
- Exclusivity of permissions is validated at the `RouteProtectionMiddleware.cs` middleware.
- Every sensible interaction, failure, or manipulation reflects as an entry inside the **Audit Service**, where IP addresses are typically stored in a pseudo-anonymized pattern (AES-256 IP masking).
