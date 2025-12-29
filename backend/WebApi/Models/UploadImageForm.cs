using Microsoft.AspNetCore.Http;

namespace WebApi.Models
{
    public class UploadImageForm
    {
        public IFormFile File { get; set; } = default!;
    }
}
