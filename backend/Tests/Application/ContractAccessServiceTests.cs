using FluentAssertions;
using InvoiceGenerator.Api.Application.Abstractions;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Exceptions;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Domain.Entities;
using InvoiceGenerator.Api.Domain.Interfaces;
using Moq;
using Xunit;

namespace InvoiceGenerator.Api.Tests.Application
{
    public sealed class ContractAccessServiceTests
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<ICurrentUserAccessor> _user = new();
        private readonly Mock<IRepository<Contract>> _contracts = new();

        public ContractAccessServiceTests()
        {
            _uow.Setup(x => x.Contracts).Returns(_contracts.Object);
        }

        [Fact]
        public void EnsureCanAccessContract_WhenContractNull_ShouldThrow404()
        {
            _user.Setup(x => x.IsAdmin()).Returns(false);
            var sut = new ContractAccessService(_uow.Object, _user.Object);

            var act = () => sut.EnsureCanAccessContract(null);

            act.Should().Throw<ApiException>()
                .Where(e => e.StatusCode == 404 && e.Message == ApiResponseMessages.ContractNotFound);
        }

        [Fact]
        public void EnsureCanAccessContract_WhenAdmin_ShouldNotThrow()
        {
            _user.Setup(x => x.IsAdmin()).Returns(true);
            var sut = new ContractAccessService(_uow.Object, _user.Object);
            var c = new Contract { Id = Guid.NewGuid(), OwnerUserId = Guid.NewGuid() };

            var act = () => sut.EnsureCanAccessContract(c);

            act.Should().NotThrow();
        }

        [Fact]
        public void EnsureCanAccessContract_WhenUserIsOwner_ShouldNotThrow()
        {
            var uid = Guid.NewGuid();
            _user.Setup(x => x.IsAdmin()).Returns(false);
            _user.Setup(x => x.GetUserId()).Returns(uid);
            var sut = new ContractAccessService(_uow.Object, _user.Object);
            var c = new Contract { Id = Guid.NewGuid(), OwnerUserId = uid };

            var act = () => sut.EnsureCanAccessContract(c);

            act.Should().NotThrow();
        }

        [Fact]
        public void EnsureCanAccessContract_WhenUserNotOwner_ShouldThrow404()
        {
            _user.Setup(x => x.IsAdmin()).Returns(false);
            _user.Setup(x => x.GetUserId()).Returns(Guid.NewGuid());
            var sut = new ContractAccessService(_uow.Object, _user.Object);
            var c = new Contract { Id = Guid.NewGuid(), OwnerUserId = Guid.NewGuid() };

            var act = () => sut.EnsureCanAccessContract(c);

            act.Should().Throw<ApiException>()
                .Where(e => e.StatusCode == 404 && e.Message == ApiResponseMessages.ContractNotFound);
        }

        [Fact]
        public async Task RequireAccessibleContractAsync_ShouldReturnContractWhenAllowed()
        {
            var cid = Guid.NewGuid();
            var uid = Guid.NewGuid();
            var c = new Contract { Id = cid, OwnerUserId = uid };
            _contracts.Setup(x => x.GetByIdAsync(cid)).ReturnsAsync(c);
            _user.Setup(x => x.IsAdmin()).Returns(false);
            _user.Setup(x => x.GetUserId()).Returns(uid);
            var sut = new ContractAccessService(_uow.Object, _user.Object);

            var result = await sut.RequireAccessibleContractAsync(cid, CancellationToken.None);

            result.Should().BeSameAs(c);
        }
    }
}
