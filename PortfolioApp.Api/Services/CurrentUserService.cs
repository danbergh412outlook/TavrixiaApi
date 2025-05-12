using System.Security.Claims;

namespace PortfolioApp.Api.Services
{
    public class CurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !(user.Identity?.IsAuthenticated ?? false))
                return null;

            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}
