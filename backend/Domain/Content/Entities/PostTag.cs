// Domain/Content/Entities/PostTag.cs
using Domain.Content.Taxonomy;

namespace Domain.Content.Entities
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public Guid TagId { get; set; }
        public Tag? Tag { get; set; }
    }
}
