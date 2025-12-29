using Application.Taxonomy.DTOs;
using MediatR;

namespace Application.Taxonomy.Commands.CreateTag
{
    public record CreateTagCommand(CreateTagRequest Request) : IRequest<TagDto>;
}
