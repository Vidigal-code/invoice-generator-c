# invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

Welcome to **invoice-generator-c**, an enterprise-grade Debt Simulation & Agreement Formalization White-Label platform. This repository contains a full-stack monolithic-scaled solution, strictly decoupled through Docker containers.

### 🏛 Architecture Overview
- **Backend (.NET 9 C#):** Hosted in the `/backend` directory. Conceived around **Clean Architecture**, **DDD (Domain-Driven Design)**, and **CQRS (via MediatR)**. Includes high-tier security protocols such as Anti-DDoS Rate Limiting, strict CSPs, unit-of-work state management, and HttpOnly Secure Cookies for authentication. It features comprehensive TDD coverage (Unit and Integration testing using Moq and xUnit).
- **Frontend (Angular 22):** Hosted in the `/frontend` directory. Built under a Standalone component-based architecture emphasizing high scalability. It implements reactive state management via **Signals and RxJS**, wrapped in a 100% responsive **Dark Blue** layout driven by Angular Material.
- **Database (SQL Server):** Fully containerized using `docker-compose.yml`, initialized seamlessly via an SQL tracking script.

**Quick Start:** `docker compose up --build`
</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

Bem-vindo ao **invoice-generator-c**, uma plataforma corporativa e White-Label para Simulação de Dívidas e Formalização de Acordos. Este repositório entrega uma solução ponta-a-ponta robusta, dissociada por containeres Docker.

### 🏛 Orquestração da Arquitetura
- **Backend (.NET 9 C#):** Localizado no diretório `/backend`. Arquitetado com **Clean Architecture**, **DDD (Domain-Driven Design)** e **CQRS (via MediatR)** para separação pura de obrigações (SOLID). Incorpora defesas rigorosas como Limitadores de Taxa Anti-DDoS (Rate Limiting), HSTS (Strict-Transport-Security), Cookies de Autenticação *HttpOnly* (blindando proteção contra roubos de sessão) e design fortemente fundamentado por Testes Ágeis Integrados e Unitários (TDD puro com Moq/xUnit). Conta com uma Trilha de Auditoria (AuditLog DLP) automatizada capaz de mascarar dados sensíveis PII por Regex e gerar rastreios transacionais por IPs. Todo o ecossistema C# utiliza Comentários em Bloco JSDoc Bilíngues (EN/PT) integrados ao container do **Dual Swagger UI**.
- **Frontend (Angular 22):** Encontrado em `/frontend`. Construído em cima do arquétipo de Componentes Standalone com a máxima otimização reativa extraída de **Signals e RxJS**. Apresenta rastreabilidade HTTP fortemente alicerçada via comentários block JSDoc nativos. Dispõe de uma experiência de tela com tema "Azul Escuro", puramente responsiva à diferentes resoluções via Flex e Menus dinâmicos de tipo Hambúrguer. Implanta cabeçalhos autônomos `X-Correlation-ID` em cada requisição para alimentar as matrizes de rastreio da API.
- **Banco de Dados (SQL Server):** Orquestrado instantaneamente através de imagens em `docker-compose`, dispensando as instalações vitais de ambiente para dev.

**Inicio Rápido:** `docker compose up --build`
</details>
