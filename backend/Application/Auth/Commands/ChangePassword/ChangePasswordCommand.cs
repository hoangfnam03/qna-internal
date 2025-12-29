using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.ChangePassword
{
    public record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<bool>;
}
