using FluentValidation;
using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Domain.Validation;
using InvoiceGenerator.Api.Infrastructure.Validation;

namespace InvoiceGenerator.Api.Application.Validators
{
    public sealed class CreateContractCommandValidator : AbstractValidator<CreateContractCommand>
    {
        public CreateContractCommandValidator()
        {
            RuleFor(x => x.ContractNumber).NotEmpty().MaximumLength(50).AntiXssAndSqli();
            RuleFor(x => x.DebtorName).NotEmpty().MaximumLength(255).AntiXssAndSqli();
            RuleFor(x => x.DebtorDocument)
                .NotEmpty()
                .MaximumLength(50)
                .AntiXssAndSqli()
                .Must(doc =>
                {
                    var d = CpfValidator.NormalizeDigits(doc);
                    if (d.Length != 11)
                        return true;
                    return CpfValidator.IsValid(doc);
                })
                .WithMessage(ApiResponseMessages.InvalidCpf);
            RuleFor(x => x.OriginalValue).GreaterThan(0);
            RuleFor(x => x.CurrentBalance).GreaterThanOrEqualTo(0);
            RuleFor(x => x.InstallmentsCount)
                .InclusiveBetween(1, NegotiationLimits.MaxInstallments)
                .WithMessage(ApiResponseMessages.InvalidInstallmentsCount);
        }
    }
}
