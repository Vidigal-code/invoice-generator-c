# Database Schema and Relationships

**Invoice Generator C** handles rigorous financial entity relationships ensuring traceability. Built atop `Entity Framework Core`, the database enforces strict navigational properties.

## Relational Entity-Resource Diagram

```mermaid
erDiagram
    TENANT ||--o{ USER : manages
    TENANT ||--o{ DEBT : issues
    DEBT ||--|{ PORTFOLIO : contains
    DEBT ||--o{ CONTRACT : generates
    CONTRACT ||--|{ CONTRACT_HISTORY : registers
    USER ||--o{ AUDIT_LOG : tracks
    
    TENANT {
        uuid Id PK
        string Name
        string CNPJ "Unique Index"
    }
    
    DEBT {
        uuid Id PK
        uuid TenantId FK
        decimal GrossAmount
        datetime CreatedAt
        boolean IsFormalized
    }
    
    CONTRACT {
        uuid Id PK
        uuid DebtId FK
        string PaymentMethod
        decimal ApprovedValue
        datetime FormalizationDate
    }
    
    CONTRACT_HISTORY {
        uuid Id PK
        uuid ContractId FK
        string Action "Enum (Created, Paid, Locked)"
        datetime Timestamp
    }
    
    AUDIT_LOG {
        uuid Id PK
        uuid UserId FK
        string Endpoint
        string MaskedIP "AES-256 encrypted suffix"
        string JsonPayload "Anonymized payload"
    }
```

## Entity Framework Behavior

- **Lazy Loading is Disabled**: To prevent N+1 connection storms, EF Core strictly mandates `.Include()` query structures when hydrating parent-child contexts (e.g., pulling `Contract` and fetching its appended `ContractHistory` nodes).
- **Audit Interceptors**: EF Core Interceptors inject implicit fields like `CreatedAt` and `UpdatedAt` right before committing changes onto the execution pipeline. 
- **Soft Deletions**: Financial records are never truly purged (`DELETE FROM`). Operations toggle an internal structural `IsActive` or `DeletedAt` timestamp, rendering records oblivious to typical queries unless explicitly searched through Admin panels.
