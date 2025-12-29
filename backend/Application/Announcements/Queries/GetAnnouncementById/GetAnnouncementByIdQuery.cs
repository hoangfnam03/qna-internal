using Application.Announcements.DTOs;
using MediatR;

namespace Application.Announcements.Queries.GetAnnouncementById
{
    public record GetAnnouncementByIdQuery(Guid Id, bool IncludeUnpublished = false) : IRequest<AnnouncementDetailDto>;
}
