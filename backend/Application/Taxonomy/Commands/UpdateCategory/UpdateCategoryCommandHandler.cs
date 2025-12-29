using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.Taxonomy.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly IApplicationDbContext _db;
        public UpdateCategoryCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var entity = await _db.Categories.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) throw new InvalidOperationException("Category not found.");

            var name = request.Request.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Name is required.");

            var slug = string.IsNullOrWhiteSpace(request.Request.Slug) ? Slugify.From(name) : Slugify.From(request.Request.Slug!);
            if (string.IsNullOrWhiteSpace(slug)) throw new InvalidOperationException("Slug is required.");

            if (request.Request.ParentId == entity.Id)
                throw new InvalidOperationException("ParentId cannot be itself.");

            if (request.Request.ParentId.HasValue)
            {
                var parentExists = await _db.Categories.AsNoTracking().AnyAsync(x => x.Id == request.Request.ParentId.Value, ct);
                if (!parentExists) throw new InvalidOperationException("Parent category not found.");
            }

            var slugTaken = await _db.Categories.AsNoTracking()
                .AnyAsync(x => x.Slug == slug && x.Id != entity.Id, ct);
            if (slugTaken) throw new InvalidOperationException("Category slug already exists.");

            entity.Name = name;
            entity.Slug = slug;
            entity.ParentId = request.Request.ParentId;
            entity.SortOrder = request.Request.SortOrder;
            entity.IsHidden = request.Request.IsHidden;

            await _db.SaveChangesAsync(ct);

            return new CategoryDto(entity.Id, entity.Name, entity.Slug, entity.ParentId, entity.SortOrder, entity.IsHidden);
        }
    }
}
