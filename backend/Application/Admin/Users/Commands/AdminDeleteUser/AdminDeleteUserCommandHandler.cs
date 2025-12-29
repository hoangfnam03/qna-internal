using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Admin.Users.Commands.AdminDeleteUser
{
    public class AdminDeleteUserCommandHandler : IRequestHandler<AdminDeleteUserCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        private readonly ICurrentUserService _current;

        public AdminDeleteUserCommandHandler(IApplicationDbContext db, ICurrentUserService current)
        {
            _db = db; _current = current;
        }

        public async Task<bool> Handle(AdminDeleteUserCommand request, CancellationToken ct)
        {
            if (_current.UserId == null) throw new UnauthorizedAccessException();

            var user = await _db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == request.UserId, ct);
            if (user == null) throw new InvalidOperationException("User not found.");

            // Soft delete
            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
