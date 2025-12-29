using Application.Moderation.DTOs;
using MediatR;

namespace Application.Moderation.Commands.AdminDeleteTarget
{
    public record AdminDeleteTargetCommand(DeleteTargetRequest Request) : IRequest<bool>;
}
