using MediatR;

namespace Application.Announcements.Commands.PublishAnnouncement
{
    public record PublishAnnouncementCommand(Guid Id) : IRequest<bool>;
}
