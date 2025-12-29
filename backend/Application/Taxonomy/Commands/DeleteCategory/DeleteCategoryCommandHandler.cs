using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly IApplicationDbContext _db;
        public DeleteCategoryCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) return true;

            var hasChildren = await _db.Categories.AsNoTracking().AnyAsync(x => x.ParentId == entity.Id, ct);
            if (hasChildren) throw new InvalidOperationException("Cannot delete category with children.");

            var usedByPosts = await _db.Posts.AsNoTracking().AnyAsync(p => p.CategoryId == entity.Id, ct);
            if (usedByPosts) throw new InvalidOperationException("Cannot delete category used by posts.");

            _db.Categories.Remove(entity);
            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
