using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    public class UserSurvey
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserEmail { get; set; }
        [Required]
        public int SurveyId { get; set; }
        [Required]
        public DateTime DateTaken { get; set; }

        public Survey Survey { get; set; }
        public ICollection<UserResponse> UserResponse { get; set; }
    }
}
