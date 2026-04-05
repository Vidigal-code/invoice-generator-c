# Frontend UI - invoice-generator-c

## 🇺🇸 English Description
<details>
<summary><strong>View Details</strong></summary>

The Frontend portal for the **invoice-generator-c** debt simulation platform. Fully built using **Angular 22** implementing a strictly modular, Standalone Components-based "Layered Architecture". 

- **Modern UX:** Features a dark blue color palette to induce trustworthiness.
- **Fully Responsive:** Completely collapses into centralized cards and utilizes a side "Hamburger Menu" for smaller viewports (Mobile-first concepts).
- **Reactive State:** Makes extensive use of `RxJS` streams and Angular `Signals` to manage API side-effects globally without code duplication.
- **Robustness:** Strong usage of HTTP Interceptors seamlessly attaching secure JWT logic using HttpOnly cross-origin cookie requirements, bypassing local-storage vulnerabilities.
</details>

## 🇧🇷 Descrição em Português
<details>
<summary><strong>Ver Detalhes</strong></summary>

O portal Frontend para plataforma de simulação de dívidas do **invoice-generator-c**, redigido modernamente em **Angular 22** focado no modelo Standalone e limpo de arquitetura em camadas visando escalabilidade Enterprise.

- **Traceability Transparente:** Dispara dinamicamente cabeçalhos assíncronos `X-Correlation-ID` em todas as rotas do client usando Crypto `UUID`. Isso garante o lastro para investigações unificadas no console do Backend (Monitoramento Fintech-Scale).
- **Segurança Bancária:** Interceptors desenhados para delegar Sessões e JWTs 100% sob sigilo nativo de navegadores via `withCredentials`. Impossibilitando extrações locais feitas por malwares ou injeções de tela (XSS protection nativa aliada aos cabaçalhos de HSTS restritos consumidos do backend).
- **Estado Reativo (Signals & RxJS):** Focado no arquétipo Redux de gerência local sob a leveza bruta dos novos `Signals`, mitigando side effects assíncronos de chamadas isoladas da rede.
</details>
