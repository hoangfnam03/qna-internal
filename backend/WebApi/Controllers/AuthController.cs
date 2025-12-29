using Application.Auth.Commands.ChangePassword;
using Application.Auth.Commands.ForgotPassword;
using Application.Auth.Commands.Login;
using Application.Auth.Commands.Logout;
using Application.Auth.Commands.Refresh;
using Application.Auth.Commands.ResetPassword;
using Application.Auth.Commands.SetPasswordFromInvite;
using Application.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) { _mediator = mediator; }

        [HttpPost("invite/set-password")]
        [AllowAnonymous]
        public async Task<ActionResult> SetPasswordFromInvite(SetPasswordFromInviteRequest req, CancellationToken ct)
        {
            var ok = await _mediator.Send(new SetPasswordFromInviteCommand(req), ct);
            return Ok(new { ok });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login(LoginRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new LoginCommand(req), ct));

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new RefreshCommand(req), ct));

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout(LogoutRequest req, CancellationToken ct)
        {
            var ok = await _mediator.Send(new LogoutCommand(req), ct);
            return Ok(new { ok });
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest req, CancellationToken ct)
        {
            var ok = await _mediator.Send(new ChangePasswordCommand(req), ct);
            return Ok(new { ok });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new ForgotPasswordCommand(req), ct));

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordRequest req, CancellationToken ct)
        {
            var ok = await _mediator.Send(new ResetPasswordCommand(req), ct);
            return Ok(new { ok });
        }
    }
}
