using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Contracts;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Queries;
using InvoiceGenerator.Api.Application.Services.Contracts;
using InvoiceGenerator.Api.Domain.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class ContractsController : ControllerBase
    {
        private readonly IContractService _contracts;
        private readonly IMediator _mediator;

        public ContractsController(IContractService contracts, IMediator mediator)
        {
            _contracts = contracts;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] bool activeOnly = false,
            [FromQuery] int page = 1,
            [FromQuery] int size = 20)
        {
            var query = new ContractListQuery(search, status, activeOnly, page, size);
            return Ok(await _contracts.ListAsync(query));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id) =>
            Ok(await _contracts.GetByIdAsync(id));

        [HttpGet("{id:guid}/history")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> GetHistory(Guid id) =>
            Ok(await _mediator.Send(new GetContractHistoryQuery(id)));

        [HttpPost]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateContractCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { Id = id, Message = ApiResponseMessages.ContractCreated });
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ContractUpdateRequest request)
        {
            await _contracts.UpdateAsync(id, request);
            return Ok(new { Message = ApiResponseMessages.ContractUpdated });
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = ApplicationRoles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _contracts.DeleteAsync(id);
            return Ok(new { Message = ApiResponseMessages.ContractCancelled });
        }
    }
}
