using Application.Common.Interfaces;
using Application.Taxonomy.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Queries.GetCategories
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetCategoriesQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        {
            var q = _db.Categories.AsNoTracking();

            if (!request.IncludeHidden)
                q = q.Where(x => !x.IsHidden);

            return await q
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .Select(x => new CategoryDto(x.Id, x.Name, x.Slug, x.ParentId, x.SortOrder, x.IsHidden))
                .ToListAsync(ct);
        }
    }
}
