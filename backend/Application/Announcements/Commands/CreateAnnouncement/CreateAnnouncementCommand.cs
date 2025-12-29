using Application.Announcements.DTOs;
using MediatR;

namespace Application.Announcements.Commands.CreateAnnouncement
{
    public record CreateAnnouncementCommand(CreateAnnouncementRequest Request) : IRequest<Guid>;
}
