using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.Refresh
{
    public record RefreshCommand(RefreshRequest Request) : IRequest<AuthResponse>;
}
