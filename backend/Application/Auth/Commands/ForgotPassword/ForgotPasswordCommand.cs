using Application.Auth.DTOs;
using MediatR;

namespace Application.Auth.Commands.ForgotPassword
{
    public record ForgotPasswordCommand(ForgotPasswordRequest Request) : IRequest<ForgotPasswordResponse>;
}
