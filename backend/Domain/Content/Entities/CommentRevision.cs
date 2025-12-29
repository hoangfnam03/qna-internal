// Domain/Content/Entities/CommentRevision.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class CommentRevision : EntityBase
    {
        public Guid CommentId { get; set; }
        public Comment? Comment { get; set; }

        public string? BeforeBody { get; set; }
        public string? AfterBody { get; set; }
        public string? Summary { get; set; }

        public Guid EditedByUserId { get; set; }
        public User? EditedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
