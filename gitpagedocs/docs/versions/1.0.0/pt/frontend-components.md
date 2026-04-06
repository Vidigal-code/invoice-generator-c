# Componentes do Frontend

O Frontend do **Invoice Generator C** é concebido sobre Angular 17. O foco é uma interface limpa, escalável e focada na experiência em dispositivos multiplataforma.

## Padrão de Estilização
Utilizamos extensivamente o **Angular Material**. Tivemos um rigor na customização (no `styles.scss`) com suporte minucioso ao `Modo Escuro (Dark Mode)`. Elementos complexos, como tabelas de histórico ou o Menu de Navegação Global e Footer renderizam as cores dinâmicas adaptativas corretas na inversão de temas.

## Módulos Principais

### `DashboardComponent`
O nó central de visualização do cliente autônomo.
- **Portfólios:** Apresenta via `MatTable` as díridas abertas do devedor e gerencia agrupamento.
- **Botões e Dialogs:** Formalização e pagamentos resultam no popup de loading contínuo até a resposta backend via RabbitMQ Event ou polling rápido.
- **Pipes Customizados:** Formatação de Valores da Moeda e Conversão de Documento (CPF/CNPJ).

### `BilletViewerComponent`
O componente responsável por mostrar a ficha final.
- Reconhecimento automático se o anexo vindo através do LocalStack S3 é renderizável via URL encriptada ou iframe puro (PDF ou HTML).
- Tem um _sandbox_ forte e mecanismos nativos contra _Clickjacking_ ou Download Forçado sem prévio preview.

### `AdminLogsComponent` e Moderação
Gestão por trás das cortinas da plataforma.
- Tabela com filtros combinados (Nome do Devedor, Log do Sistema, ID).
- Paginação otimizada com debounce-time.
- Status Chips informativos codificados por cores dependendo dos eventos e de _IP Traces_ registrados.
