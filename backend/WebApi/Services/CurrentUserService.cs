using System.Security.Claims;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace WebApi.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserService(IHttpContextAccessor http) => _http = http;

        public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated == true;

        public Guid? UserId
        {
            get
            {
                var user = _http.HttpContext?.User;
                if (user == null) return null;

                var sub = user.FindFirstValue("sub") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(sub, out var id) ? id : null;
            }
        }

        public string? Email =>
            _http.HttpContext?.User?.FindFirstValue("email")
            ?? _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
