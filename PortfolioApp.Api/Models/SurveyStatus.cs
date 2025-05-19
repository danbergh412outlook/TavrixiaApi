using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    public class SurveyStatus
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<Survey> Surveys { get; set; }
    }
}
