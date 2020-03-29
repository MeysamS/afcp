using System.ComponentModel.DataAnnotations;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}