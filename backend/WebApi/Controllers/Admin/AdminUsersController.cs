using Application.Admin.Users.Commands.AdminInviteUser;
using Application.Admin.Users.Queries.AdminGetUsers;
using Application.Auth.DTOs;
using Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/users")]
    [Authorize(Roles = "Admin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminUsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string? search,
            [FromQuery] UserStatus? status,
            [FromQuery] bool includeDeleted = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken ct = default)
            => Ok(await _mediator.Send(new AdminGetUsersQuery(search, status, includeDeleted, page, pageSize), ct));

        [HttpPost("invite")]
        public async Task<ActionResult<InviteUserResponse>> Invite(InviteUserRequest req, CancellationToken ct)
            => Ok(await _mediator.Send(new AdminInviteUserCommand(req), ct));
    }
}
