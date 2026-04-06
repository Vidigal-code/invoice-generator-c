using FluentValidation;
using MediatR;
using System.Text.RegularExpressions;

namespace InvoiceGenerator.Api.Infrastructure.Validation
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                    throw new ValidationException(failures);
            }
            return await next();
        }
    }

    public static class CustomValidators
    {
        // Simple Anti-XSS and SQLi Pattern matching
        public static IRuleBuilderOptions<T, string> AntiXssAndSqli<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(value =>
            {
                if (string.IsNullOrEmpty(value)) return true;
                
                var hasHtmlTags = Regex.IsMatch(value, @"<.*?>");
                var hasSqlWords = Regex.IsMatch(value, @"(?i)(select|drop|insert|update|delete|table|grant|alter|exec)"); // Basic mockup for app layer
                
                return !hasHtmlTags && !hasSqlWords;
            }).WithMessage("Input contains invalid or dangerous characters representing XSS or SQLi attempts.");
        }
    }
}
