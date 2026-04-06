# Esquemas Relacionales de Tablas y Base de Datos

**Invoice Generator C** modela cimientos inquebrantables garantizando historial irrefutable. Edificada lógicamente arriba del motor de `Entity Framework Core`, blinda y dicta comportamientos de dependencias entre cada tabla.

## Diagrama Entidad-Relación (Relacional)

```mermaid
erDiagram
    TENANT ||--o{ USER : agremia
    TENANT ||--o{ DEBT : posee
    DEBT ||--|{ PORTFOLIO : ramifica
    DEBT ||--o{ CONTRACT : sella
    CONTRACT ||--|{ CONTRACT_HISTORY : narra
    USER ||--o{ AUDIT_LOG : somete
    
    TENANT {
        uuid Id PK
        string Empresa
        string CNPJ "Identificador Único"
    }
    
    DEBT {
        uuid Id PK
        uuid TenantId FK
        decimal ImporteOriginario
        datetime AltaRegistro
        boolean FormalizadoStatus
    }
    
    CONTRACT {
        uuid Id PK
        uuid DebtId FK
        string FormatoDeCobro "PIX"
        decimal AcordadoLiquido
        datetime FirmaOficial
    }
    
    CONTRACT_HISTORY {
        uuid Id PK
        uuid ContractId FK
        string Suceso "Emisión, Cobro"
        datetime InstanteCrono
    }
    
    AUDIT_LOG {
        uuid Id PK
        uuid UserId FK
        string PeticionEndpoint
        string IpDestrozada "Protección AES-256"
        string JsonData "Volcado Opaco"
    }
```

## Estrictos Dictámenes de Comportamiento EF Core

- **Restricción y Abolición del Lazy Loading (Cargas Diferidas)**: Amputada por defecto en favor preventivo ante ataques desmesurados a la red conocidos por desastres N+1 Query. Llama y demanda uniones limpias utilizando la variante cruda de mandatos `.Include()`.
- **Rastros mediante EF Interceptors**: Contadores de tiempo inyectan sobre la raíz del context antes de cada volcado (Save) sus respectivas firmas de actualización cronológica asilando al sistema dependiente manual.
- **Borradores Invisibles (Soft-Deletes)**: Una regla vital es denegar terminantemente ejecuciones absolutas `DELETE FROM`. Alterando un campo sombra vital, la fila asume exclusión aparente escondiendo sus datos solo de la vista al cliente normal pero preservándola para auditorías.
