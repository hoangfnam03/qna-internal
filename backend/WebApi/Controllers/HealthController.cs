using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("/healthz")]
        public IActionResult Health() => Ok(new { ok = true });
    }
}
