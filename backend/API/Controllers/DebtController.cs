using InvoiceGenerator.Api.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class DebtController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DebtController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{contractId}/calculate")]
        public async Task<IActionResult> CalculateDebt(Guid contractId)
        {
            var result = await _mediator.Send(new GetDebtCalculationQuery(contractId));
            return Ok(result);
        }
    }
}
