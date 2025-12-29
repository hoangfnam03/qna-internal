using Application.Announcements.DTOs;
using Application.Common.Models;
using MediatR;

namespace Application.Announcements.Queries.AdminGetAnnouncements
{
    public record AdminGetAnnouncementsQuery(bool PublishedOnly = false, int Page = 1, int PageSize = 50)
        : IRequest<Paged<AnnouncementListItemDto>>;
}
