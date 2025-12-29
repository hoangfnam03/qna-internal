using Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/me")]
    [Authorize]
    public class MeController : ControllerBase
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public MeController(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db;
            _current = current;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken ct)
        {
            if (_current.UserId == null) return Unauthorized();

            var uid = _current.UserId.Value;

            var user = await _db.Users.AsNoTracking()
                .Where(u => u.Id == uid)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.DisplayName,
                    u.Bio,
                    u.AvatarUrl,
                    u.Status,
                    u.SuspendedUntil,
                    u.SuspensionReason,
                    u.CreatedAt,
                    u.UpdatedAt
                })
                .FirstOrDefaultAsync(ct);

            if (user == null) return Unauthorized();

            var roles = await (from ur in _db.UserRoles.AsNoTracking()
                               join r in _db.Roles.AsNoTracking() on ur.RoleId equals r.Id
                               where ur.UserId == uid
                               select r.Name)
                               .ToListAsync(ct);

            return Ok(new
            {
                user,
                roles
            });
        }
    }
}
