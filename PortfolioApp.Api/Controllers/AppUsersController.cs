using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Services;

namespace PortfolioApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AppUsersController : ControllerBase
    {
        private readonly IAppUserService _appUsersService;

        public AppUsersController(IAppUserService appUsersService)
        {
            _appUsersService = appUsersService;
        }

        [HttpPost]
        public async Task<ActionResult> Create()
        {
            var user = await _appUsersService.GetOrCreateUser();
            return Ok(user);
        }
    }
}
