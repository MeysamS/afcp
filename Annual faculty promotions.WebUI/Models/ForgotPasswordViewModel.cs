using System.ComponentModel.DataAnnotations;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "آدرس ایمیل وارد نشده است!")]
        [EmailAddress(ErrorMessage = "آدرس ایمیل معتبر نیست!")]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }
    }
}