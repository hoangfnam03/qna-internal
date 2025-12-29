using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.DeleteTag
{
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        public DeleteTagCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<bool> Handle(DeleteTagCommand request, CancellationToken ct)
        {
            var entity = await _db.Tags.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) return true;

            var used = await _db.PostTags.AsNoTracking().AnyAsync(x => x.TagId == entity.Id, ct);
            if (used) throw new InvalidOperationException("Cannot delete tag used by posts.");

            _db.Tags.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
