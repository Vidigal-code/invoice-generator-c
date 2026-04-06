using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class AgreementsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgreementsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "FormalizeAgreements")]
        public async Task<IActionResult> CreateAgreement([FromBody] CreateAgreementCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateAgreement), new { id = result.Id }, result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory() =>
            Ok(await _mediator.Send(new GetAgreementHistoryQuery()));
    }
}
