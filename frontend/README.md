# Frontend UI - invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

The **Angular 19** SPA for **invoice-generator-c**. The app uses **standalone components**, **lazy-loaded routes**, and **Angular Material** for UI primitives.

### Project layout

| Area | Contents |
|------|-----------|
| **`src/app/core/`** | Route **guards** (`auth`, `admin`, `unauth`), **HTTP interceptors** (JWT bearer from API behavior + **`credentialsInterceptor`** with **`withCredentials: true`** and per-request **`X-Correlation-ID`**), **`ApiService`** and feature **API** wrappers, mappers, **Brazil-native** `DateAdapter`, password validator, shared constants |
| **`src/app/features/`** | **`home`**, **`auth`** (login / register), **`dashboard`** (debt flow + **billet viewer**), **`admin`** (operational logs, dialogs for contracts/users) |
| **`src/app/shared/`** | **Layout** shell, **navbar**, **footer**, reusable **dialog** components, **`portfolio-label`** pipe |
| **`src/app/state/`** | Lightweight shared **signals**-oriented app state |

### Tooling & config

- **CLI / framework:** `@angular/core` **19.2.x** (see `package.json`).  
- **Environments:** `src/environments/`; **`npm run sync-env`** (via `scripts/sync-env-from-root.mjs`) copies variables from the **monorepo root** `.env` before **`ng serve`** / **`ng build`**.  
- **Tests:** **Karma** + **Jasmine** (`npm run test`); specs under `src/tests/` (unit + integration-style component tests).  
- **Container:** production image builds the app and serves static files with **nginx** (see `Dockerfile` and `nginx-frontend.conf`); Compose passes build args such as API base URL.

### How to run

From **`frontend/`**:

```bash
npm install
npm start
```

Ensure the API CORS settings allow your dev origin (e.g. `http://localhost:4200`) and that the root **`.env`** exists if you rely on **`sync-env`**.

</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

SPA **Angular 19** do **invoice-generator-c**, com **componentes standalone**, **rotas lazy-loaded** e **Angular Material**.

### Estrutura

- **`core/`** — **guards**, **interceptors** (JWT + credenciais e cabeçalho **`X-Correlation-ID`** com `crypto.randomUUID()`), serviços HTTP, validações, constantes.  
- **`features/`** — **`home`**, **`auth`**, **`dashboard`** (visualização de boletos), **`admin`**.  
- **`shared/`** — **layout**, **navbar**, **footer**, diálogos reutilizáveis.  
- **`state/`** — estado partilhado baseado em **signals**.

### Ferramentas

Versões em **`package.json`** (Angular **19.2**). Script **`sync-env`** sincroniza o **`.env`** da **raiz do monorepo** antes de **`ng serve`** / **`ng build`**. Testes com **Karma/Jasmine**; imagem **Docker** com **nginx** em produção.

### Execução

```bash
cd frontend
npm install
npm start
```

Configurar CORS na API para o origin de desenvolvimento (ex.: `http://localhost:4200`).

</details>
