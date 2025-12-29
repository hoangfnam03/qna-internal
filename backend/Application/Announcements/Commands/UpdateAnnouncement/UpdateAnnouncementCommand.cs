using Application.Announcements.DTOs;
using MediatR;

namespace Application.Announcements.Commands.UpdateAnnouncement
{
    public record UpdateAnnouncementCommand(Guid Id, UpdateAnnouncementRequest Request) : IRequest<bool>;
}
