using Application.Files.Commands.UploadImage;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/files")]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public FilesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("images")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageForm form, CancellationToken ct)
            => Ok(await _mediator.Send(new UploadImageCommand(form.File), ct));
    }
}
