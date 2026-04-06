# Frontend Components

The Frontend of **Invoice Generator C** is architected on Angular 17. The focus remains on an impeccably clean, highly scalable interface centered on a multiplatform experience.

## Styling Standard
We extensively rely on **Angular Material**. We applied exhaustive customizations (inside `styles.scss`) offering meticulous support for `Dark Mode`. Complex elements — like historical tables or the Global Navigation Menu and the Footer — fluidly cast their correct dynamic adaptive shades during theme shifts.

## Main Modules

### `DashboardComponent`
The primary pivot for standalone client interaction.
- **Portfolios:** Propagates the debtor’s open debts grouping them with a robust `MatTable`.
- **Buttons and Dialogs:** The agreement and payment requests open a loading gateway spinner right until the backend fires a resolution flag via RabbitMQ Events or fast-polling.
- **Custom Pipes:** Responsible for Formatting Currency Outputs and masked documents parsing (CPF/CNPJ).

### `BilletViewerComponent`
The specific display widget to project the finalized payment slit.
- Automatic sniffing to decide whether the returned content sourced securely via LocalStack S3 URL warrants an IFrame or pure HTML rendering format.
- Features potent sandbox mechanics alongside native protections buffering out _Clickjacking_ flaws or uncontrolled download forces without displaying an immediate prior preview.

### `AdminLogsComponent` and Backend Ops
Pivots the operational backstage settings of the platform.
- Full filter-combined matrix sets (Search by Debtor Name, System Event Log, Identifier ID).
- Pre-Debouncer-Driven Pagination loops.
- Colored informative tags highlighting exact status statuses combined directly with traces of IP origins.
