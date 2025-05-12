using Microsoft.AspNetCore.Mvc;

namespace PortfolioApp.Api.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected string? UserEmail => User.FindFirst("email")?.Value;
    }
}
