using FluentValidation;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Validation;

namespace InvoiceGenerator.Api.Application.Validators
{
    public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Must(StrongPasswordPolicy.IsSatisfied)
                .WithMessage(ApiResponseMessages.WeakPassword);
        }
    }
}
