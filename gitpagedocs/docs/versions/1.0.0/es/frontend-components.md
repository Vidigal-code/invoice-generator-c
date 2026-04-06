# Componentes del Frontend

El Frontend de **Invoice Generator C** está concebido bajo Angular 17. El enfoque prioritario es una interfaz pulida y altamente escalable para el cruce de interacción moderna en diferentes dispositivos.

## Estándar de Estilo
Acudimos intensamente de la mano de **Angular Material**. Mantuvimos rigurosidad en personalización puntual en la carga del enrutador de `styles.scss`, con amplio énfasis de acomodación automática para `Dark Mode (Modo Oscuro)`. Complementos exhaustivos, interacciones laterales con Tablas Históricas, y colores adaptativos en el Menú y Pie de Página actúan inteligentemente gracias esta homogeneidad de estilo.

## Módulos Principales

### `DashboardComponent`
Es el punto central que dirige la manipulación para el usuario de negocio.
- **Portafolios/Tablas:** Presenta las deudas vigentes bajo controles responsivos agrupando elementos mediante `MatTable`.
- **Botones y Cuadros de Selección:** Las ejecuciones de solicitud de pagos gatillan cajas modales continuas (Spinners) reaccionadas por RabbitMQ Events o rutinas asíncronas agilizadas del lado servidor.
- **Tuberías Adaptadas (Custom Pipes):** Formatación para visualización monetaria limpia junto al encapsulamiento de datos confidenciales como Documentos de Identidad Universales (CPF/CNPJ).

### `BilletViewerComponent`
Ente delegado exclusivamente para el control de la papeleta finalizada en pantalla.
- Reconoce orgánicamente si el material desprendido como URL desde el almacenamiento S3 (LocalStack) se interpreta con formato estático nativo (PDF) o interactivo directo encajado en iFrame o formato HTML sin interrupción.
- Emplea directivas _sandbox_ duras, conteniendo tácticas de control estricto que paralizan problemas de _Clickjacking_ o descargas ocultas subyacentes.

### `AdminLogsComponent` y Operaciones
Lógicamente enriquece funciones subyacentes y controles globales.
- Presentaciones segmentadas en un panel con filtros variables (Nombre, Log, y el ID).
- Ejecución aplazada o inteligente de consultas de paginación limitando cargas inútiles.
- Etiquetas Visuales pre-determinadas (Status Chips) categorizadas a partir de colorizaciones descriptivas, apuntando trazas fidedignas (Registros directos basados en IPs operacionales).
