using InvoiceGenerator.Api.Application.Commands;
using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Services.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IMediator _mediator;

        public AuthController(IAuthService auth, IMediator mediator)
        {
            _auth = auth;
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _auth.LoginAsync(request, ip);

            Response.Cookies.Append(AuthCookieConstants.TokenCookieName, result.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(AuthCookieConstants.SessionHours)
            });

            return Ok(new { result.Token, result.Role, result.Username, Message = ApiResponseMessages.Authenticated });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var userId = await _mediator.Send(new RegisterCommand(request.Username, request.Password, request.Email));
            return CreatedAtAction(nameof(Login), new { id = userId }, new { Message = ApiResponseMessages.RegistrationSuccess });
        }
    }
}
