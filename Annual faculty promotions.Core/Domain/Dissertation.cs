using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    // راهنمای پروژه ، پایان نامه، و رساله دانشجویی خاتمه یافته
    public class Dissertation:AuditableEntity<long>
    {
        // شماره درخواست
        public long RequestId { get; set; }
        public virtual Request Request { get; set; }

        // عنوان
        public string Subject { get; set; }

        // نام دانشجو
        public string StudentName { get; set; }

        // مقطع تخصیلی
        public GradeEducation GradeEducation { get; set; }

        // تاریخ شروع
        public string BeginDate { get; set; }

        // تاریخ دفاع
        public string VindicationDate { get; set; }

        // تعداد واحد
        public int UnitCount { get; set; }

        // واحد معادل
        public float UnitEqual { get; set; }

    }
}
