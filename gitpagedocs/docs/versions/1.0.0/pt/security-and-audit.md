# Paradigmas de Segurança e Foco em Auditoria

A Segurança é inquebrável, regendo o núcleo do **Invoice Generator C**. Devido às exigências estritas das legislações em transferências de capital e acordo, orquestramos barreiras gigantes de controle RBAC e rastreadores forenses imutáveis.

## 1. Entrincheiramento de Papéis (RBAC) 

As sessões firmam a entrega de Claims via JWT sem reter tramas perigosas com estado no Servidor.
- **RouteProtectionMiddleware**: O cão de guarda supremo imposto nos pipelines `.NET`. Destroem ou validam imediatamente quem demanda por uma rota baseada não em hierarquia universal, mas pela relação da claim vs end-point exigido originando puros e duros retornos `HTTP 403 Forbidden` aos negados.
- **Escudo Multitenant (Multilocatário)**: Uma permissão de Supervisor é castrada de ofício se sua ação mira contratos originados noutros inquilinos (Tenants divergentes do seu id). Evita vazamentos horizontais por força bruta incalculável.

## 2. Mascaramento IP Irreversiblemente Reversível (AES-256)

Proteção generalizada de dados é o mantra. Ips puros não coabitam nas linhas de auditorias puras.
Ao passarem pelas portas da rotina do `AuditService`, as metragens do IP perdem seus octetos terminantemente sendo imediatamente trituradas com cifra **AES-256 Pura**.

`192.0.2.146` -> `192.0.*.* [U2FsdGVkX1.../aB]`

Este disfarce legalista isenta o sistema de violações de dados ao reter dados primários. Contudo, portando a Chave Suprema injetada do Servidor Oculto é passível revelar a raiz para esferas contenciosas ou de mitigação severas e exploração rastreada judicial.

## 3. Bastião contra Perigos XSS/CSRF

Muitos modelos de Autenticadores Single Page tombam falhos frente capturas em Javascript maléficas.
- **Vácuo XSS**: O Token vital transita do Back devolvido amarrado num cofre *Cookie Component* taxado em pedra no protocolo da Web de não aceitar cópias JS, o aclamado `HttpOnly`. O Angular se torna puramente passante, ignorando tokens que repousam seguros no cofre do Client Browser.
- **Barricada SameSite**: Empurradas exclusivas sobre tráfegos de blindagem SSL (HTTPS) com diretrizes fortificadas exigindo correspondências cegas de restrições em `SameSite=Strict` anulando saltos em páginas de domínios mal intencionados cruzados externamente (Blindando CSRF).
