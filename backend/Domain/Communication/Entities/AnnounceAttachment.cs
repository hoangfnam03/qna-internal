// Domain/Communication/Entities/AnnouncementAttachment.cs
using Domain.Files.Entities;

namespace Domain.Communication.Entities
{
    public class AnnouncementAttachment
    {
        public Guid AnnouncementId { get; set; }
        public Announcement? Announcement { get; set; }

        public Guid FileId { get; set; }
        public FileObject? File { get; set; }
    }
}
