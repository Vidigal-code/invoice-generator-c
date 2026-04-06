using FluentValidation;
using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Validation;
using InvoiceGenerator.Api.Infrastructure.Validation;

namespace InvoiceGenerator.Api.Application.Validators
{
    public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .AntiXssAndSqli();

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(255)
                .AntiXssAndSqli();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Must(StrongPasswordPolicy.IsSatisfied)
                .WithMessage(ApiResponseMessages.WeakPassword);
        }
    }
}
