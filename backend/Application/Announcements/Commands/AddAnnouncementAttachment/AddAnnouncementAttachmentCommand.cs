using MediatR;

namespace Application.Announcements.Commands.AddAnnouncementAttachment
{
    public record AddAnnouncementAttachmentCommand(Guid AnnouncementId, Guid FileId) : IRequest<bool>;
}
