using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Taxonomy.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Queries.GetTags
{
    public class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Paged<TagDto>>
    {
        private readonly IApplicationDbContext _db;
        public GetTagsQueryHandler(IApplicationDbContext db) => _db = db;

        public async Task<Paged<TagDto>> Handle(GetTagsQuery request, CancellationToken ct)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = request.PageSize is < 1 or > 200 ? 20 : request.PageSize;

            var q = _db.Tags.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var s = request.Search.Trim().ToLowerInvariant();
                q = q.Where(x => x.Name.ToLower().Contains(s) || x.Slug.ToLower().Contains(s));
            }

            var total = await q.CountAsync(ct);

            var items = await q
                .OrderBy(x => x.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new TagDto(x.Id, x.Name, x.Slug))
                .ToListAsync(ct);

            return new Paged<TagDto>(items, page, pageSize, total);
        }
    }
}
