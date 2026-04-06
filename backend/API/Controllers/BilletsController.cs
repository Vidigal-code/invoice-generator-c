using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public sealed class BilletsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BilletsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{billetId}/pdf")]
        public async Task<IActionResult> DownloadPdf(System.Guid billetId)
        {
            var bytes = await _mediator.Send(new GetBilletPdfQuery(billetId));
            if (IsPdf(bytes))
                return File(bytes, HttpContentTypes.Pdf, $"Boleto_{billetId}.pdf");
            return File(bytes, HttpContentTypes.HtmlUtf8, $"Boleto_{billetId}.html");
        }

        private static bool IsPdf(byte[] bytes) =>
            bytes.Length >= 4
            && bytes[0] == (byte)'%'
            && bytes[1] == (byte)'P'
            && bytes[2] == (byte)'D'
            && bytes[3] == (byte)'F';
    }
}
