using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Services;

namespace PortfolioApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class UserSurveysController : ControllerBase
    {
        private readonly IUserSurveyService _userSurveyService;

        public UserSurveysController(IUserSurveyService userSurveyService)
        {
            _userSurveyService = userSurveyService;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUserSurveyDto dto)
        {
            var id = await _userSurveyService.CreateUserSurveyAsync(dto);
            return Ok(id);
        }
        [HttpGet("{surveyUrlName}")]
        public async Task<ActionResult<SurveyDto>> GetSurveyWithUserInfo(
            string surveyUrlName,
            [FromQuery] bool errorNotFound,
            [FromQuery] bool currentUser,
            [FromQuery] string? userUrlName
            )
        {
            var result = await _userSurveyService.GetUserSurveyDtoAsync(
                surveyUrlName,
                currentUser,
                userUrlName);

            if (result == null)
                return errorNotFound ? NotFound() : Ok(null);

            return Ok(result);
        }
    }
}
