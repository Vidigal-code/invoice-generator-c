# Detalhes do Frontend (Angular)

O frontend do Invoice Generator é desenvolvido com **Angular (v18+)** e conta com uma arquitetura robusta seguindo as melhores práticas modernas.

## Componentes Centrais da Arquitetura
- **Framework**: Angular
- **Design de Componentes**: Standalone Components e Feature-Sliced Design (Design Segmentado por Funcionalidades).
- **Estrização**: TailwindCSS e Angular Material.
- **Gerenciamento de Estado**: Estado Reativo / Serviços com Signals e/ou RxJS.

## Módulos Principais
1. **Módulo Core**: Serviços Singleton, Interceptors (ex.: AuthInterceptor para injetar tokens JWT) e configurações globais da aplicação.
2. **Módulo Shared**: Componentes reutilizáveis (botões, janelas de diálogo, spinners), pipes e diretivas.
3. **Módulos de Funcionalidade (Features)**: Voltados as regras de negócios, como Faturas (Invoices), Configurações, Autenticação e Perfis de Usuários.

## Rotas e Guards (Proteções)
O sistema de roteamento utiliza "Lazy Loading" para otimizar o tempo de carregamento inicial. Ele aplica `AuthGuard` e `RoleGuard` para bloquear o acesso não autorizado de forma dinâmica (RBAC).

## Integração (Networking)
Todas as chamadas à API são estritamente tipadas através de interfaces TypeScript que refletem os DTOs do backend. Os HTTP Interceptors lidam com o tratamento centralizado de erros e o envio/renovação de tokens JWT.
