CREATE DATABASE InvoiceGeneratorC;
GO

USE InvoiceGeneratorC;
GO

-- 1. Users and Roles (RBAC)
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(255) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL
);

CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    RoleId UNIQUEIDENTIFIER NOT NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- 2. Audit Logs
CREATE TABLE AuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL,
    Action NVARCHAR(100) NOT NULL,
    EntityName NVARCHAR(100) NOT NULL,
    EntityId NVARCHAR(100) NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    IpAddress NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- 3. Core Domain: Contracts and Installments (Portfolio = WalletPortfolioJson.Value / DebtCalculation key)
CREATE TABLE Contracts (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    OwnerUserId UNIQUEIDENTIFIER NULL,
    ContractNumber NVARCHAR(50) NOT NULL UNIQUE,
    DebtorName NVARCHAR(255) NOT NULL,
    DebtorDocument NVARCHAR(50) NOT NULL,
    OriginalValue DECIMAL(18,2) NOT NULL,
    CurrentBalance DECIMAL(18,2) NOT NULL,
    Portfolio NVARCHAR(50) NOT NULL DEFAULT 'invoice-generator-c',
    Status NVARCHAR(50) NOT NULL DEFAULT 'Active',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Contracts_Users_Owner FOREIGN KEY (OwnerUserId) REFERENCES Users(Id)
);

CREATE TABLE Installments (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ContractId UNIQUEIDENTIFIER NOT NULL,
    InstallmentNumber INT NOT NULL,
    DueDate DATE NOT NULL,
    OriginalValue DECIMAL(18,2) NOT NULL,
    CurrentValue DECIMAL(18,2) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Open',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Installments_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id)
);

CREATE TABLE ContractHistories (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ContractId UNIQUEIDENTIFIER NOT NULL,
    ChangedByUserId UNIQUEIDENTIFIER NULL,
    ChangeType NVARCHAR(50) NOT NULL,
    PayloadJson NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT FK_ContractHistories_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id)
);

-- 4. Agreements and Billets
CREATE TABLE Agreements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    ContractId UNIQUEIDENTIFIER NOT NULL,
    TotalNegotiatedValue DECIMAL(18,2) NOT NULL,
    InstallmentsCount INT NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Agreements_Contracts FOREIGN KEY (ContractId) REFERENCES Contracts(Id)
);

CREATE TABLE Billets (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    AgreementId UNIQUEIDENTIFIER NOT NULL,
    InstallmentNumber INT NOT NULL,
    DueDate DATE NOT NULL,
    Value DECIMAL(18,2) NOT NULL,
    Barcode NVARCHAR(100) NOT NULL,
    PdfUrl NVARCHAR(500) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Billets_Agreements FOREIGN KEY (AgreementId) REFERENCES Agreements(Id)
);
GO

INSERT INTO Roles (Id, Name, Description) VALUES ('C7D5C5E0-0000-0000-0000-000000000001', 'Admin', 'Total System Control');
INSERT INTO Roles (Id, Name, Description) VALUES ('C7D5C5E0-0000-0000-0000-000000000002', 'User', 'Standard User');
GO

-- Admin user é criado pelo seed da API a partir de AdminSettings (.env: ADMIN_USERNAME, ADMIN_EMAIL, ADMIN_PASSWORD)

-- Contrato demo legado (compatível com testes que usam CT-2026-001)
DECLARE @LegacyId UNIQUEIDENTIFIER = '11111111-1111-1111-1111-111111111111';
IF NOT EXISTS(SELECT 1 FROM Contracts WHERE Id = @LegacyId)
BEGIN
    INSERT INTO Contracts (Id, ContractNumber, DebtorName, DebtorDocument, OriginalValue, CurrentBalance, Portfolio, Status, CreatedAt)
    VALUES (@LegacyId, 'CT-2026-001', 'João Silva', '52998224725', 1500.00, 1500.00, 'invoice-generator-c', 'Active', GETUTCDATE());
    INSERT INTO Installments (Id, ContractId, InstallmentNumber, DueDate, OriginalValue, CurrentValue, Status, CreatedAt)
    VALUES (NEWID(), @LegacyId, 1, '2026-05-10', 500.00, 500.00, 'Open', GETUTCDATE()),
           (NEWID(), @LegacyId, 2, '2026-06-10', 500.00, 500.00, 'Open', GETUTCDATE()),
           (NEWID(), @LegacyId, 3, '2026-07-10', 500.00, 500.00, 'Open', GETUTCDATE());
END
GO

-- Contratos para fluxo completo (dívida → acordo → boletos); admin do .env
DECLARE @P1 UNIQUEIDENTIFIER = '22222222-2222-2222-2222-222222222222';
IF NOT EXISTS(SELECT 1 FROM Contracts WHERE Id = @P1)
BEGIN
    INSERT INTO Contracts (Id, ContractNumber, DebtorName, DebtorDocument, OriginalValue, CurrentBalance, Portfolio, Status, CreatedAt)
    VALUES (@P1, 'IGC-2026-100', 'Maria Silva', '39053344705', 2400.00, 2400.00, 'invoice-generator-c', 'Active', GETUTCDATE());
    INSERT INTO Installments (Id, ContractId, InstallmentNumber, DueDate, OriginalValue, CurrentValue, Status, CreatedAt)
    VALUES (NEWID(), @P1, 1, DATEADD(DAY, -20, CAST(GETUTCDATE() AS DATE)), 800.00, 800.00, 'Open', GETUTCDATE()),
           (NEWID(), @P1, 2, DATEADD(DAY, -5, CAST(GETUTCDATE() AS DATE)), 800.00, 800.00, 'Open', GETUTCDATE()),
           (NEWID(), @P1, 3, DATEADD(DAY, 30, CAST(GETUTCDATE() AS DATE)), 800.00, 800.00, 'Open', GETUTCDATE());
END
GO

DECLARE @P2 UNIQUEIDENTIFIER = '33333333-3333-3333-3333-333333333333';
IF NOT EXISTS(SELECT 1 FROM Contracts WHERE Id = @P2)
BEGIN
    INSERT INTO Contracts (Id, ContractNumber, DebtorName, DebtorDocument, OriginalValue, CurrentBalance, Portfolio, Status, CreatedAt)
    VALUES (@P2, 'IGC-2026-101', 'Carlos Negociação', '11144477735', 900.00, 900.00, 'invoice-generator-c', 'Active', GETUTCDATE());
    INSERT INTO Installments (Id, ContractId, InstallmentNumber, DueDate, OriginalValue, CurrentValue, Status, CreatedAt)
    VALUES (NEWID(), @P2, 1, DATEADD(DAY, -45, CAST(GETUTCDATE() AS DATE)), 450.00, 450.00, 'Open', GETUTCDATE()),
           (NEWID(), @P2, 2, DATEADD(DAY, 10, CAST(GETUTCDATE() AS DATE)), 450.00, 450.00, 'Open', GETUTCDATE());
END
GO
