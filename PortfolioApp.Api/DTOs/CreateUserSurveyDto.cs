using PortfolioApp.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.DTOs
{
    public class CreateUserSurveyDto
    {
        public string SurveyUrlName { get; set; }
        public List<CreateUserResponseDto> UserResponses { get; set; }
    }
}
