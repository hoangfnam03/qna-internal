using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.Logout
{
    public record LogoutCommand(LogoutRequest Request) : IRequest<bool>;
}
