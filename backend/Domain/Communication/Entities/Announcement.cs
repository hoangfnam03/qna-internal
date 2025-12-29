// Domain/Communication/Entities/Announcement.cs
using Domain.Common.Entities;
using Domain.Identity.Entities;

namespace Domain.Communication.Entities
{
    public class Announcement : SoftDeletableEntity
    {
        public string Title { get; set; } = default!;
        public string? Body { get; set; }

        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }

        public new Guid CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }

        public new Guid? UpdatedByUserId { get; set; }
        public User? UpdatedByUser { get; set; }

        public ICollection<AnnouncementAttachment> Attachments { get; set; } = new List<AnnouncementAttachment>();
    }
}
