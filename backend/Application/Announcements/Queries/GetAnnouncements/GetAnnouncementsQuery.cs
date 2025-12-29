using Application.Announcements.DTOs;
using Application.Common.Models;
using MediatR;

namespace Application.Announcements.Queries.GetAnnouncements
{
    public record GetAnnouncementsQuery(int Page = 1, int PageSize = 20) : IRequest<Paged<AnnouncementListItemDto>>;
}
