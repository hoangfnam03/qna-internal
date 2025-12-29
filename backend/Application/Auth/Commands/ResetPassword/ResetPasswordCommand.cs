using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.ResetPassword
{
    public record ResetPasswordCommand(ResetPasswordRequest Request) : IRequest<bool>;
}
