namespace InvoiceGenerator.Api.Application.Constants
{
    public static class ApiResponseMessages
    {
        public const string LoggingPathNotConfigured =
            "Logging:FilePath não configurado (mesmo caminho usado pelo Serilog).";

        public const string NoLogFileFound = "Nenhum log encontrado.";
        public const string UserNotFound = "Usuário não encontrado.";
        public const string InvalidRole = "Role inválida.";
        public const string UserUpdated = "Usuário atualizado com sucesso.";
        public const string PasswordReset = "Senha redefinida com sucesso.";
        public const string ContractNotFound = "Contrato não encontrado.";
        public const string ContractNumberExists = "Número de contrato já existe.";
        public const string ContractUpdated = "Contrato atualizado com sucesso.";
        public const string ContractCancelled = "Contrato cancelado com sucesso.";
        public const string ContractFormalizationLocked = "Contrato está a ser processado; tente novamente dentro de instantes.";
        public const string AgreementAlreadyActive = "Este contrato já possui um acordo formalizado ativo.";
        public const string InvalidInstallmentsCount = "Quantidade de parcelas inválida (permitido: 1 a 24).";
        public const string InvalidCredentials = "Credenciais inválidas";
        public const string UsernameExists = "Usuário já existe.";
        public const string RegistrationSuccess = "Usuário registrado com sucesso.";
        public const string Authenticated = "Authenticated successfully";
        public const string BilletNotAvailable = "Boleto não disponível.";
        public const string NotApplicable = "N/A";
        public const string WeakPassword =
            "A senha deve ter no mínimo 8 caracteres, incluindo maiúscula, minúscula, número e caractere especial.";
        public const string InvalidCpf = "CPF inválido.";
        public const string ValidationFailed = "Dados inválidos.";
        public const string ForbiddenContractOperation = "Operação não permitida neste contrato.";
        public const string ContractCreated = "Contrato criado com sucesso.";
    }
}
