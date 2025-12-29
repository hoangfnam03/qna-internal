using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.Taxonomy.DTOs;
using Domain.Content.Taxonomy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.CreateCategory
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IApplicationDbContext _db;
        public CreateCategoryCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var r = request.Request;

            var name = r.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Name is required.");

            var slug = string.IsNullOrWhiteSpace(r.Slug) ? Slugify.From(name) : Slugify.From(r.Slug!);
            if (string.IsNullOrWhiteSpace(slug)) throw new InvalidOperationException("Slug is required.");

            var exists = await _db.Categories.AsNoTracking().AnyAsync(x => x.Slug == slug, ct);
            if (exists) throw new InvalidOperationException("Category slug already exists.");

            if (r.ParentId.HasValue)
            {
                var parentExists = await _db.Categories.AsNoTracking().AnyAsync(x => x.Id == r.ParentId.Value, ct);
                if (!parentExists) throw new InvalidOperationException("Parent category not found.");
            }

            var entity = new Category
            {
                Name = name,
                Slug = slug,
                ParentId = r.ParentId,
                SortOrder = r.SortOrder,
                IsHidden = r.IsHidden
            };

            _db.Categories.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new CategoryDto(entity.Id, entity.Name, entity.Slug, entity.ParentId, entity.SortOrder, entity.IsHidden);
        }
    }
}
