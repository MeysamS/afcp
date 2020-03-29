using System.ComponentModel.DataAnnotations;

namespace Annual_faculty_promotions.WebUI.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; } 
        [Display(Name = "نام لاتین")]
        [Required(ErrorMessage = "نام وارد نشده است!", AllowEmptyStrings = false)]
        public string Name { get; set; }

         [Display(Name = "نام فارسی")]
        public string PersianName { get; set; }
    }
}