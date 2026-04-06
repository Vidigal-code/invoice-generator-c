using InvoiceGenerator.Api.Domain.Authorization;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using MediatR;

namespace InvoiceGenerator.Api.Application.Commands
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Guid>
    {
        private readonly IUnitOfWork _uow;
        private readonly IApplicationRoleNames _roleNames;

        public RegisterCommandHandler(IUnitOfWork uow, IApplicationRoleNames roleNames)
        {
            _uow = uow;
            _roleNames = roleNames;
        }

        public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await _uow.Users.FindAsync(u => u.Username == request.Username);
            if (existingUser.Any())
                throw new ApiException(400, ApiResponseMessages.UsernameExists);

            var roles = await _uow.Roles.FindAsync(r => r.Name == _roleNames.User);
            var userRole = roles.FirstOrDefault();
            Guid roleId;

            if (userRole == null)
            {
                roleId = Guid.NewGuid();
                await _uow.Roles.AddAsync(new Role
                {
                    Id = roleId,
                    Name = _roleNames.User,
                    Description = "Default User Role"
                });
            }
            else
            {
                roleId = userRole.Id;
            }

            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Email = request.Email,
                RoleId = roleId,
                IsActive = true
            };

            await _uow.Users.AddAsync(newUser);
            return newUser.Id;
        }
    }
}
