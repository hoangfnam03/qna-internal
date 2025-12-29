using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.Taxonomy.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.UpdateTag
{
    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand, TagDto>
    {
        private readonly IApplicationDbContext _db;
        public UpdateTagCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<TagDto> Handle(UpdateTagCommand request, CancellationToken ct)
        {
            var entity = await _db.Tags.FirstOrDefaultAsync(x => x.Id == request.Id, ct);
            if (entity == null) throw new InvalidOperationException("Tag not found.");

            var name = request.Request.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Name is required.");

            var slug = string.IsNullOrWhiteSpace(request.Request.Slug) ? Slugify.From(name) : Slugify.From(request.Request.Slug!);
            if (string.IsNullOrWhiteSpace(slug)) throw new InvalidOperationException("Slug is required.");

            var slugTaken = await _db.Tags.AsNoTracking()
                .AnyAsync(x => x.Slug == slug && x.Id != entity.Id, ct);
            if (slugTaken) throw new InvalidOperationException("Tag slug already exists.");

            entity.Name = name;
            entity.Slug = slug;

            await _db.SaveChangesAsync(ct);

            return new TagDto(entity.Id, entity.Name, entity.Slug);
        }
    }
}
