using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Services;
using System.Security.Claims;

namespace PortfolioApp.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class SurveysController : ControllerBase
    {
        private readonly SurveyService _surveyService;

        public SurveysController(SurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: /surveys
        [HttpGet]
        public async Task<ActionResult<List<SurveyDto>>> GetAll()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            return Ok(surveys);
        }
        [HttpGet("{urlName}")]
        public async Task<ActionResult<SurveyDetailsDto>> GetSurveyByUrlName(string urlName)
        {
            var result = await _surveyService.GetSurveyWithQuestionsByUrlNameAsync(urlName);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: /surveys
        [HttpPost]
        public async Task<ActionResult<SurveyDto>> Create([FromBody] UpdateSurveyDto dto)
        {
            var created = await _surveyService.UpdateSurveyAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
        }
        [HttpPut("{urlName}")]
        public async Task<IActionResult> UpdateSurvey(string urlName, [FromBody] UpdateSurveyDto dto)
        {
            var updatedSurvey = await _surveyService.UpdateSurveyAsync(dto);
            return Ok(updatedSurvey);
        }
    }
}
