using Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Files.Commands.UploadImage
{
    public record UploadImageCommand(IFormFile File) : IRequest<FileDto>;
}
