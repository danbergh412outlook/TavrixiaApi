using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    public class ResponseStatus
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; }
    }
}
