// Domain/Content/Entities/CommentAttachment.cs
using Domain.Common.Entities;
using Domain.Files.Entities;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class CommentAttachment : EntityBase
    {
        public Guid CommentId { get; set; }
        public Comment? Comment { get; set; }

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
