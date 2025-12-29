// Domain/Content/Taxonomy/Tag.cs
using Domain.Common.Entities;
using Domain.Content.Entities;

namespace Domain.Content.Taxonomy
{
    public class Tag : SoftDeletableEntity
    {
        public string Name { get; set; } = default!;
        public string Slug { get; set; } = default!;

        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}
