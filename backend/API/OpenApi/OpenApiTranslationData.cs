using Microsoft.OpenApi.Models;

namespace InvoiceGenerator.Api.API.OpenApi;

/// <summary>
/// Localised OpenAPI copy: English (<see cref="OpenApiDocumentNames.V1En"/>) and Brazilian Portuguese (<see cref="OpenApiDocumentNames.V1Br"/>).
/// OperationIds follow <c>CustomOperationIds</c> in <c>Program.cs</c> ({Controller}_{Action}).
/// </summary>
public static class OpenApiTranslationData
{
    public static OpenApiInfo GetApiInfo(string documentName) =>
        documentName == OpenApiDocumentNames.V1Br ? ApiInfoBr() : ApiInfoEn();

    public static List<OpenApiTag> GetTags(string documentName) =>
        documentName == OpenApiDocumentNames.V1Br ? TagsBr() : TagsEn();

    public static bool TryGetOperation(string documentName, string operationId, out string summary, out string? description)
    {
        if (!Operations.TryGetValue(operationId, out var pair))
        {
            summary = string.Empty;
            description = null;
            return false;
        }

        var br = documentName == OpenApiDocumentNames.V1Br;
        summary = br ? pair.SummaryBr : pair.SummaryEn;
        description = br ? pair.DescBr : pair.DescEn;
        return true;
    }

    public static void ApplyAuthResponseDescriptions(OpenApiOperation operation, string documentName)
    {
        if (operation.Security == null || operation.Security.Count == 0)
            return;

        var br = documentName == OpenApiDocumentNames.V1Br;
        if (operation.Responses.TryGetValue("401", out var u))
            u.Description = br
                ? "Não autorizado — cookie AuthToken ausente ou inválido. Use POST /api/Auth/login."
                : "Unauthorized — missing or invalid AuthToken cookie. Call POST /api/Auth/login.";
        if (operation.Responses.TryGetValue("403", out var f))
            f.Description = br
                ? "Proibido — autenticado, mas sem permissão para este recurso ou política."
                : "Forbidden — authenticated but not allowed for this resource or policy.";
    }

    private static OpenApiInfo ApiInfoEn() => new()
    {
        Title = "Invoice Generator API — English",
        Version = "v1",
        Description = """
            HTTP API for **invoice-generator-c** (debt simulation and agreement formalization).

            ### Authentication
            - **HttpOnly cookie** `AuthToken` (JWT; JWE supported per server configuration).
            - After `POST /api/Auth/login`, the browser sends the cookie on the same host (including Swagger UI / ReDoc on this service).
            - Non-browser clients: send `Cookie: AuthToken=<token>` after login.

            ### Recommended headers
            - `X-Correlation-ID`: UUID for end-to-end tracing (also echoed in responses).

            ### Limits and security
            - Global rate limiting and strict headers apply to API routes; `/docs` and `/swagger` use a relaxed CSP so documentation UIs load.

            ### OpenAPI specs
            - This document: **English** (`v1-en`). Brazilian Portuguese: `v1-br` at `/swagger/v1-br/swagger.json`.
            """
    };

    private static OpenApiInfo ApiInfoBr() => new()
    {
        Title = "Invoice Generator API — Português (Brasil)",
        Version = "v1",
        Description = """
            API HTTP da plataforma **invoice-generator-c** (simulação de dívidas e formalização de acordos).

            ### Autenticação
            - **Cookie HttpOnly** `AuthToken` (JWT; suporte a JWE conforme configuração do servidor).
            - Após `POST /api/Auth/login`, o navegador envia o cookie no mesmo host (incluindo Swagger UI / ReDoc neste serviço).
            - Clientes fora do navegador: envie `Cookie: AuthToken=<token>` após o login.

            ### Cabeçalhos recomendados
            - `X-Correlation-ID`: UUID para rastreamento ponta a ponta (também retornado na resposta).

            ### Limites e segurança
            - Rate limiting global e cabeçalhos rígidos valem para a API; `/docs` e `/swagger` usam CSP flexível para carregar as UIs de documentação.

            ### Especificações OpenAPI
            - Este documento: **pt-BR** (`v1-br`). Inglês: `v1-en` em `/swagger/v1-en/swagger.json`.
            """
    };

    private static List<OpenApiTag> TagsEn() =>
    [
        new OpenApiTag
        {
            Name = "Auth",
            Description = "Authentication and user registration. Sets the `AuthToken` cookie on login."
        },
        new OpenApiTag
        {
            Name = "Contracts",
            Description = "Portfolio contracts: list, detail, history (admin), create, update, and cancel."
        },
        new OpenApiTag
        {
            Name = "Debt",
            Description = "Debt calculation and installment projection for an authorized contract."
        },
        new OpenApiTag
        {
            Name = "Agreements",
            Description = "Agreement formalization and history for the signed-in user."
        },
        new OpenApiTag
        {
            Name = "Billets",
            Description = "Download boleto as PDF or fallback HTML depending on storage."
        },
        new OpenApiTag
        {
            Name = "AdminPanel",
            Description = "Admin operations: system logs, audit trail, users, passwords, roles. Requires **Admin** role."
        }
    ];

    private static List<OpenApiTag> TagsBr() =>
    [
        new OpenApiTag
        {
            Name = "Auth",
            Description = "Autenticação e cadastro de usuários. Define o cookie `AuthToken` no login."
        },
        new OpenApiTag
        {
            Name = "Contracts",
            Description = "Contratos da carteira: listagem, detalhe, histórico (admin), criação, atualização e cancelamento."
        },
        new OpenApiTag
        {
            Name = "Debt",
            Description = "Cálculo da dívida e projeção de parcelas para um contrato autenticado."
        },
        new OpenApiTag
        {
            Name = "Agreements",
            Description = "Formalização de acordos e histórico do usuário autenticado."
        },
        new OpenApiTag
        {
            Name = "Billets",
            Description = "Download do boleto em PDF ou HTML de contingência conforme o armazenamento."
        },
        new OpenApiTag
        {
            Name = "AdminPanel",
            Description = "Operações administrativas: logs, auditoria, usuários, senhas e papéis. Exige papel **Admin**."
        }
    ];

    private static readonly Dictionary<string, OperationCopy> Operations = new(StringComparer.Ordinal)
    {
        ["Auth_Login"] = new(
            "Sign in",
            "Authenticates with username and password. Returns JSON with token, role, and username, and sets the **HttpOnly** `AuthToken` cookie (Secure, SameSite=Strict).",
            "Fazer login",
            "Autentica com nome de usuário e senha. Retorna JSON com token, papel e nome de usuário, e define o cookie **HttpOnly** `AuthToken` (Secure, SameSite=Strict)."),
        ["Auth_Register"] = new(
            "Register account",
            "Creates a user with a strong password and unique email. Returns `201 Created`; does not open a session — call login afterward.",
            "Registrar conta",
            "Cria um usuário com senha forte e e-mail único. Resposta `201 Created`; não abre sessão — faça login em seguida."),
        ["Contracts_GetAll"] = new(
            "List contracts",
            "Paginated list with optional filters: search text, status, activeOnly, `page`, and `size`. Requires authentication.",
            "Listar contratos",
            "Lista paginada com filtros opcionais: texto de busca, status, apenas ativos, `page` e `size`. Requer autenticação."),
        ["Contracts_GetById"] = new(
            "Get contract by id",
            "Returns full contract details if the user is allowed to access that contract.",
            "Obter contrato por ID",
            "Retorna o detalhe do contrato se o usuário tiver permissão de acesso."),
        ["Contracts_GetHistory"] = new(
            "Contract change history",
            "Chronological list of changes/events (**Admin** only).",
            "Histórico de alterações do contrato",
            "Lista cronológica de eventos/alterações (somente **Admin**)."),
        ["Contracts_Create"] = new(
            "Create contract",
            "Creates a new portfolio contract (**Admin**). Body: `CreateContractCommand`.",
            "Criar contrato",
            "Cria um novo contrato na carteira (**Admin**). Corpo: `CreateContractCommand`."),
        ["Contracts_Update"] = new(
            "Update contract",
            "Updates allowed fields (**Admin**).",
            "Atualizar contrato",
            "Atualiza campos permitidos (**Admin**)."),
        ["Contracts_Delete"] = new(
            "Cancel or remove contract",
            "Logical cancel/remove per domain rules (**Admin**).",
            "Cancelar ou remover contrato",
            "Cancelamento ou remoção lógica conforme regras de domínio (**Admin**)."),
        ["Debt_CalculateDebt"] = new(
            "Calculate contract debt",
            "Runs the configured debt strategy (installment projection, totals, metadata) for the given `contractId`.",
            "Calcular dívida do contrato",
            "Executa a estratégia de cálculo (parcelas, totais, metadados) para o `contractId` informado."),
        ["Agreements_CreateAgreement"] = new(
            "Formalize agreement",
            "Creates an agreement for a contract; requires **FormalizeAgreements** policy. May trigger async messaging.",
            "Formalizar acordo",
            "Cria um acordo vinculado ao contrato; exige política **FormalizeAgreements**. Pode disparar processamento assíncrono."),
        ["Agreements_GetHistory"] = new(
            "User agreement history",
            "Lists agreements for the authenticated user.",
            "Histórico de acordos do usuário",
            "Lista os acordos do usuário autenticado."),
        ["Billets_DownloadPdf"] = new(
            "Download boleto (PDF or HTML)",
            "Returns PDF when available; otherwise HTML fallback with boleto data.",
            "Baixar boleto (PDF ou HTML)",
            "Retorna PDF quando disponível; caso contrário, HTML de contingência com os dados do boleto."),
        ["AdminPanel_GetSystemLogs"] = new(
            "Tail application log file",
            "Reads the last `lines` lines from the configured Serilog file sink.",
            "Últimas linhas do log da aplicação",
            "Lê as últimas `lines` linhas do arquivo de log configurado (Serilog em disco)."),
        ["AdminPanel_GetAuditLogs"] = new(
            "Audit log query",
            "Paginated audit trail with optional entity and action filters.",
            "Registros de auditoria",
            "Consulta paginada da trilha de auditoria com filtros opcionais por entidade e ação."),
        ["AdminPanel_GetLoginEvents"] = new(
            "Login events",
            "Paginated authentication events for review (**Admin**).",
            "Eventos de login",
            "Lista eventos de autenticação para análise (**Admin**)."),
        ["AdminPanel_GetAgreementActions"] = new(
            "Agreement-related admin actions",
            "Paginated administrative actions tied to agreements.",
            "Ações sobre acordos",
            "Lista ações administrativas relacionadas a acordos (paginada)."),
        ["AdminPanel_GetUsers"] = new(
            "List users",
            "Lists users with optional search filter.",
            "Listar usuários",
            "Lista usuários com filtro de busca opcional."),
        ["AdminPanel_UpdateUser"] = new(
            "Update user",
            "Updates user fields per `UserUpdateRequest` (**Admin**).",
            "Atualizar usuário",
            "Atualiza dados do usuário conforme `UserUpdateRequest` (**Admin**)."),
        ["AdminPanel_ResetPassword"] = new(
            "Reset user password",
            "Sets a new password for the user (**Admin**).",
            "Redefinir senha",
            "Define uma nova senha para o usuário indicado (**Admin**)."),
        ["AdminPanel_GetRoles"] = new(
            "List roles",
            "Returns application roles (e.g. Admin, User).",
            "Listar papéis",
            "Retorna os papéis disponíveis na aplicação (ex.: Admin, User).")
    };

    private sealed record OperationCopy(string SummaryEn, string DescEn, string SummaryBr, string DescBr);
}
