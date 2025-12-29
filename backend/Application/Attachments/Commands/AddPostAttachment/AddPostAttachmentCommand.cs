using Application.Attachments.DTOs;
using MediatR;

namespace Application.Attachments.Commands.AddPostAttachment
{
    public record AddPostAttachmentCommand(Guid PostId, AddAttachmentRequest Request, bool IsAdmin) : IRequest<Guid>;
}
