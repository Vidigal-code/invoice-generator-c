# Inmersión Profunda Frontend

**Invoice Generator C** asume posiciones sólidas forjadas plenamente entorno a las ventajas y desacoples integrados base en Angular 17.

## 1. Disección y Encapsulación Modular

La red directiva recorta zonas lógicas utilizando pautas durísimas del estamento `Lazy Loading` restringiendo bultos pesados innecesarios frente descargas del consumidor de navegadores.
- `admin-logs.component` (`/admin`): Muestra una colección impecablemente armada en grillas ricas vía `MatTable` con paginado escalonado dominado activamente utilizando lógicas de _Debouncers RxJS_ (Evitando asfixiar y petardear el servidor constantemente por letras buscadas).
- `dashboard.component` (`/dashboard`): Organiza montos adeudados subsidiados, manipulando respuestas `mat-snack-bar` rastreando señales o pausas liberadas remotamente a través de _Eventos RMQ_.
- `billet-viewer.component`: Rutas hermanadas herméticas encargadas de amarrarse con enlaces presignados S3 y envolver archivos digitales estáticos esquivando brechas en la inserciones originarios del visualizador iFrame nativo.

## 2. Red y Envolturas Seguras (Autenticación Global)

Evitamos deliberadamente almacenar el vulnerable testigo global de pase (`JWT`) sobre el formato crónico e inseguro del `local-storage` para aplacar invasiones profundas directas por parte de código malicioso XSS. Nuestro puente HTTP principal únicamente asume una amarra inviolable invisible en la sesión:

```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const secureReq = req.clone({
    withCredentials: true // Delega explícitamente el repase vitalicio al back con un cofre indescifrable Cookies Secure-HttpOnly
  });
  return next(secureReq);
};
```

## 3. Trinchera De Ejecución Boletera (Sandbox)

La exhibición de cobros listos requeridos recae severamente bajo obligaciones duras para el acoplamiento hermoprotegido. Evita así contaminaciones transversales ocultando la inyección de la procedencia.

```html
<iframe
    [src]="pdfSafeUrl"
    sandbox="allow-same-origin allow-scripts"
    [title]="'Contrato Boleteria'">
</iframe>
```

## 4. Compatibilidad Global Oscurecida (Material Dark Mode)

Por requerimiento estructural fundamental, se dispuso de inmersiones orgánicas e indoloras oscurecedoras del visual. Bajo inagotables horas alterando líneas maestras incrustadas firmemente al root principal de estilos `styles.scss` recae total uniformidad:
- **Mat-Chips de Status**: Atenúa saturaciones punzantes apaciguando desgastes de pantallas extendidas.
- **Líneas Mat-Inputs**: Replantea envoltorios, márgenes y sombras iluminando discretamente bajo selectores de evento hover.
- **Relieves Profundos**: Permuta matices oscurecedores gruesos del marco transformándolo a proyecciones translúcidas tenues conservando volumen arquitectónico puro inclusive dentro de perfiles oscuros masivos profundos.
