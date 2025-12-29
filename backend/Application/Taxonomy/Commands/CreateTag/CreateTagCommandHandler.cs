using Application.Common.Interfaces;
using Application.Common.Utils;
using Application.Taxonomy.DTOs;
using Domain.Content.Taxonomy;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Taxonomy.Commands.CreateTag
{
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, TagDto>
    {
        private readonly IApplicationDbContext _db;
        public CreateTagCommandHandler(IApplicationDbContext db) => _db = db;

        public async Task<TagDto> Handle(CreateTagCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var name = r.Name.Trim();
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidOperationException("Name is required.");

            var slug = string.IsNullOrWhiteSpace(r.Slug) ? Slugify.From(name) : Slugify.From(r.Slug!);
            if (string.IsNullOrWhiteSpace(slug)) throw new InvalidOperationException("Slug is required.");

            var exists = await _db.Tags.AsNoTracking().AnyAsync(x => x.Slug == slug, ct);
            if (exists) throw new InvalidOperationException("Tag slug already exists.");

            var entity = new Tag { Name = name, Slug = slug };

            _db.Tags.Add(entity);
            await _db.SaveChangesAsync(ct);

            return new TagDto(entity.Id, entity.Name, entity.Slug);
        }
    }
}
