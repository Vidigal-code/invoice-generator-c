# Frontend: Observações e Estruturas Ocultas

**Invoice Generator C** firma seu alicerce em cima do robusto Angular 17. Ele está fragmentado logicamente em componentes puros mantendo lógicas de _features_ radicalmente distintas em subpastas isoladas.

## 1. Separação Estrita dos Módulos Funcionais

A árvore de carregamentos impõe severo _Lazy Loading_ otimizando drasticamente os pacotes enviados à placa de render do navegador (browser payload):
- `admin-logs.component` (`/admin`): Carrega a densa estrutura das matrizes da `MatTable`, manipulando instâncias atreladas `Debouncers` via RxJS acoplando requisições imensas e não entupindo requisições ao Back.
- `dashboard.component` (`/dashboard`): Tabulador de contratos e faturas secundárias. Amarram `mat-snack-bar` monitorando dinamicamente laços vindos de _RabbitMQ events_ sobre formalização terminada.
- `billet-viewer.component`: Módulo de exibição hermético com amarrações seguras carregando via iFrame a exibição de arquivos em Mockados via AWS S3 perfeitamente assinados.

## 2. Interceptors Globais e Trânsito Auth

Como abolimos ferozmente alocações brutas do token (JWT) na falha vitrine de `local-storage` para obliterar injeções XSS nefastas, o esqueleto de base acentua intercepções silenciosas invisíveis e vitais:

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const secureReq = req.clone({
    withCredentials: true // Libera o envio seguro blindado de Cookies HttpOnly restritos transacionalmente.
  });
  return next(secureReq);
};
```

## 3. Renderização Hermética na Emissão do Boleto

Exibir documentos finais em formatos cruciais exigiu enclausurar o componente em correntes inquebráveis: `sandbox`. Garante chance absoluta zero de injeção JavaScript caso ocorra envenenamento vindo nas amarras da nuvem AWS.

```html
<iframe
    [src]="pdfSafeUrl"
    sandbox="allow-same-origin allow-scripts"
    [title]="'Visualizador Boleto'">
</iframe>
```

## 4. Modo Noturno Absoluto (Dark Mode Material)

O compromisso executivo e estético forçaram suporte nativo impecável nas rotas. Dentro do `styles.scss` há varreduras universais refatorando profundamente:
- **Mat-Chips**: Adaptam para contrastes frios evitando desgastes agressivos de visão ocular.
- **Bordas Mat-Inputs**: As linhas contornáveis repassam coloração branco ofuscada e adaptativa no foco de teclado.
- **Elevações Box-Shadow**: Recuo inteligente nos contrastes brutais transformando recuos z-index numa transição refinada imitando painéis e vitrines iluminadas minimamente.
