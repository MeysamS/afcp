using Annual_faculty_promotions.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Annual_faculty_promotions.Core.Domain.User
{
    public class Profile
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام وارد نشده است!")]
        [Display(Name = "نام")]
        public string Name { get; set; }

        [Display(Name = "نام خانوادگی")]
        [Required(ErrorMessage = "نام خانوادگی وارد نشده است!")]
        public string Family { get; set; }

        [Display(Name = "تلفن")]
        [Required(ErrorMessage = "تلفن همراه وارد نشده است!")]
        public string Phone { get; set; }

        [Display(Name = "تاریخ تولد")]
        [UIHint("PersianDatePicker")]
        public DateTime? BrithDate { get; set; }

        [Display(Name = "تاریخ استخدام")]
        [Required(ErrorMessage = "تاریخ استخدام وارد نشده است!")]
        [UIHint("PersianDatePicker")]
        public DateTime EmployeeDate { get; set; }

        [Display(Name = "جنسیت")]
        [Required(ErrorMessage = "جنسیت انتخاب نشده است!")]
        public Gender Gender { get; set; }



        [Display(Name = "آخرین مدرک تحصیلی")]
        [Required(ErrorMessage = "مدرک تحصیلی وارد نشده است")]
        public GradeEducation LastGradeEducation { get; set; }

        [Required(ErrorMessage = "رشته تحصیلی وارد نشده است")]
        [Display(Name = "رشته تحصیلی")]
        public string FieldofStudy { get; set; }

        [Display(Name = "تصویر شخص")]
        public string Avatar { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}
