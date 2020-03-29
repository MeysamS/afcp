using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class RegisterViewModel
    {
         [Remote("HasValidCodeMeli", "Home", HttpMethod = "Post", ErrorMessage = "کد ملی در سیستم نامعتبر می باشد!")]
         [Display(Name = "کد ملی")]
        [Required(ErrorMessage = "کد ملی وارد نشده است!")]
        public long CodeMeli { get; set; }

         [Display(Name = "کد استخدامی")]
         [Required(ErrorMessage = "کد استخدامی وارد نشده است!")]
         [Remote("HasValidCodeEstekhdami", "Home", HttpMethod = "Post", ErrorMessage = "کد استخدامی در سیستم نامعتبر می باشد!")]
        public string CodeEstekhdam { get; set; }

        [Required(ErrorMessage = "آدرس ایمیل وارد نشده است!")]
        [EmailAddress]
        [Display(Name = "آدرس ایمیل")]
        [Remote("IsEmailExist", "Home", HttpMethod = "Post", ErrorMessage = "این آدرس ایمیل قبلا ثبت شده است!")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "آدرس ایمیل نامعتبر می باشد!")]
        public string Email { get; set; }

         [Required(ErrorMessage = "رمزعبور وارد نشده است!")]
        [StringLength(100, ErrorMessage = "حداقل کاراکترهای مجاز 6 می باشد", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "رمز عبور")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "تکرار رمز عبور")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "رمزهای عبور وارد شده باهم برابر نیستند!")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "کد امنیتی وارد نشده است")]
        [Remote("CompareCaptcha", "Home", HttpMethod = "Post", ErrorMessage = "کد امنیتی وارد شده صحیح نیست!")]
        [DisplayName("کد امنیتی")]
        public string Captcha { get; set; }
    }
}