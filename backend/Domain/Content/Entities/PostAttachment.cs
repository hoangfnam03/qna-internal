// Domain/Content/Entities/PostAttachment.cs
using Domain.Common.Entities;
using Domain.Files.Entities;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class PostAttachment : EntityBase
    {
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public Guid FileId { get; set; }
        public FileObject? File { get; set; }

        public string? Caption { get; set; }
        public string? DisplayText { get; set; }

        public int SortOrder { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
    }
}
