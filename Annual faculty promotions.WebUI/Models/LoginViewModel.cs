using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "آدرس ایمیل وارد نشده است!")]
        [Display(Name = "آدرس ایمیل")]
        [Remote("IsEmailConfirmed", "Home", HttpMethod = "Post", ErrorMessage = "حساب کاربری تائید نشده است. به ایمیل خود مراجعه کنید و نسبت به فعال سازی حساب خود اقدام نمائید")]
        [EmailAddress]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "آدرس ایمیل نامعتبر می باشد!")]
        public string Email { get; set; }

         [Required(ErrorMessage = "رمزعبور وارد نشده است!")]
        [DataType(DataType.Password)]
        [Display(Name = "رمزعبور")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }

        [Required(ErrorMessage = "کد امنیتی وارد نشده است")]
        [Remote("CompareCaptcha", "Home", HttpMethod = "Post", ErrorMessage = "کد امنیتی وارد شده صحیح نیست!")]
        [DisplayName("کد امنیتی")]
        public string Captcha { get; set; }
    }
}