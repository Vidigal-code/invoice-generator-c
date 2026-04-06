using InvoiceGenerator.Api.Application.Constants;
using InvoiceGenerator.Api.Application.DTOs.Requests;
using InvoiceGenerator.Api.Application.Services.Admin;
using InvoiceGenerator.Api.Domain.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Api.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = ApplicationRoles.Admin)]
    public sealed class AdminPanelController : ControllerBase
    {
        private readonly IAdminPanelService _adminPanel;

        public AdminPanelController(IAdminPanelService adminPanel)
        {
            _adminPanel = adminPanel;
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetSystemLogs([FromQuery] int lines = 200) =>
            Ok(await _adminPanel.GetSystemLogsAsync(lines));

        [HttpGet("audit-logs")]
        public async Task<IActionResult> GetAuditLogs(
            [FromQuery] int page = 1,
            [FromQuery] int size = 50,
            [FromQuery] string? entity = null,
            [FromQuery] string? action = null) =>
            Ok(await _adminPanel.GetAuditLogsAsync(page, size, entity, action));

        [HttpGet("login-events")]
        public async Task<IActionResult> GetLoginEvents([FromQuery] int page = 1, [FromQuery] int size = 50) =>
            Ok(await _adminPanel.GetLoginEventsAsync(page, size));

        [HttpGet("agreement-actions")]
        public async Task<IActionResult> GetAgreementActions([FromQuery] int page = 1, [FromQuery] int size = 50) =>
            Ok(await _adminPanel.GetAgreementActionsAsync(page, size));

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery] string? search) =>
            Ok(await _adminPanel.GetUsersAsync(search));

        [HttpPut("users/{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserUpdateRequest request)
        {
            await _adminPanel.UpdateUserAsync(id, request);
            return Ok(new { Message = ApiResponseMessages.UserUpdated });
        }

        [HttpPost("users/{id:guid}/reset-password")]
        public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
        {
            await _adminPanel.ResetPasswordAsync(id, request);
            return Ok(new { Message = ApiResponseMessages.PasswordReset });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles() =>
            Ok(await _adminPanel.GetRolesAsync());
    }
}
