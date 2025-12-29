using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Commands.UpdateTag
{
    public record UpdateTagCommand(Guid Id, UpdateTagRequest Request) : IRequest<TagDto>;
}
