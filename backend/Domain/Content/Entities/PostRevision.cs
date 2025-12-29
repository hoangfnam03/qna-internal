// Domain/Content/Entities/PostRevision.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;

namespace Domain.Content.Entities
{
    public class PostRevision : EntityBase
    {
        public Guid PostId { get; set; }
        public Post? Post { get; set; }

        public string? BeforeTitle { get; set; }
        public string? AfterTitle { get; set; }
        public string? BeforeBody { get; set; }
        public string? AfterBody { get; set; }

        public string? Summary { get; set; }

        public Guid EditedByUserId { get; set; }
        public User? EditedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
