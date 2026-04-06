using FluentValidation;
using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Constants;

namespace InvoiceGenerator.Api.Application.Validators
{
    public class CreateAgreementCommandValidator : AbstractValidator<CreateAgreementCommand>
    {
        public CreateAgreementCommandValidator()
        {
            RuleFor(v => v.ContractId)
                .NotEmpty().WithMessage("ContractId is required.");

            RuleFor(v => v.InstallmentsCount)
                .InclusiveBetween(1, NegotiationLimits.MaxInstallments)
                .WithMessage(ApiResponseMessages.InvalidInstallmentsCount);

            RuleFor(v => v.NegotiatedValue)
                .GreaterThan(0).WithMessage("Negotiated Value must be greater than zero.");
        }
    }
}
