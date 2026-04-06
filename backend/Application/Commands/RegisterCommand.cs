using MediatR;

namespace InvoiceGenerator.Api.Application.Commands
{
    public sealed record RegisterCommand(string Username, string Password, string Email) : IRequest<Guid>;
}
