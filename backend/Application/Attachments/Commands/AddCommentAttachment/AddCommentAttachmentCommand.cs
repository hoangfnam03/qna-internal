using Application.Attachments.DTOs;
using MediatR;

namespace Application.Attachments.Commands.AddCommentAttachment
{
    public record AddCommentAttachmentCommand(Guid CommentId, AddAttachmentRequest Request, bool IsAdmin) : IRequest<Guid>;
}
