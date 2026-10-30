# Reglas de Negocio

Aplicación de contratos de cobro, cálculo de deuda, negociación y documentos de cuotas. El backend es C#/.NET pese al sufijo -c. Los documentos y códigos son demostrativos: formalizar no registra un cobro bancario ni confirma un pago.

## Flujo y casos de fallo

1. Registro → validación de contraseña/usuario → hash → login → JWT y cookie HttpOnly/Secure/SameSite=Strict. Login también devuelve Token en JSON. Controllers y ContractAccessService aplican roles y acceso al contrato. AdminPanel administra usuarios/contratos; el catálogo HTTP está en la referencia.

2. Contrato → cuotas → cálculo por cartera. Por cuota, díasAtraso=max(0, fechaReferencia-vencimiento); multa=principal×porcentaje/100 solo con atraso; interés=principal×(tasaMensual/100/díasMes)×díasAtraso. Las tasas vienen de AppSettings; díasMes inválido usa 30. CurrentValue suma principal, multa e interés. La query usa caché y comprueba acceso.

3. La formalización valida 1–24 cuotas, adquiere Redis formalize:{contractId} durante 45 segundos y comprueba acceso/acuerdos activos. Fallo de bloqueo: 409; acuerdo activo: 400. Crea Agreement/Billets, divide el valor redondeando a dos decimales y fija vencimientos cada 30 días. QuestPDF genera los PDFs en el handler antes del evento.

4. Cada PDF va al almacenamiento; si falla el upload, el handler guarda una URL mock. El contrato pasa a Negotiated y publica AgreementFormalizedEvent. El consumer invalida caché de deuda y guarda idem:agreement:{id} en Redis durante 30 días; no genera PDFs. La consulta verifica acceso e intenta recuperar PDF con fallback HTML.

5. GenericRepository.AddAsync y UpdateAsync llaman SaveChangesAsync inmediatamente. Agreement se guarda antes del bucle PDF, cada Billet después del upload y el contrato antes del evento. No existe transacción global de formalización; un fallo puede dejar acuerdo/cuotas parciales. Inspeccione estado antes de repetir.

## Archivos relacionados

- [backend/Application/Commands/CreateAgreementCommandHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Commands/CreateAgreementCommandHandler.cs)
- [backend/Application/Consumers/AgreementFormalizedConsumer.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Consumers/AgreementFormalizedConsumer.cs)
- [backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Debt/InvoiceGeneratorCDebtCalculationStrategy.cs)
- [backend/Application/Services/Contracts/ContractAccessService.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Services/Contracts/ContractAccessService.cs)
- [backend/Application/Queries/GetBilletPdfQueryHandler.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Application/Queries/GetBilletPdfQueryHandler.cs)
- [backend/Infrastructure/Configuration/AppSettings.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Configuration/AppSettings.cs)
- [docker-compose.yml](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/docker-compose.yml)
- [backend/Infrastructure/Repositories/GenericRepository.cs](https://github.com/Vidigal-code/invoice-generator-c/blob/b1c5e2caf2d2a95fb06005b0d61d64792950207c/backend/Infrastructure/Repositories/GenericRepository.cs)

## Navegación

- [Project guide](?menu=es&lang=es&id=19)
- [Source reference](?menu=es&lang=es&id=20)
