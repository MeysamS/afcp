using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;

namespace Annual_faculty_promotions.Core.Domain
{
    // فعالیت های علمی اجرایی
    public class ScientificExecutive : AuditableEntity<int>
    {
        // شماره درخواست
        public long RequestId { get; set; }
        public virtual Request Request { get; set; }

        // عنوان
        public string Subject { get; set; }

        // مدت خدمت
        //public string LengthOfService { get; set; }

            //تاریخ شروع
        public string StartDate { get; set; }

        //تاریخ پایان
        public string EndDate { get; set; }

        // ساعات حضور در ماه
        public int TimeofMounth { get; set; }

        // امتیاز
        public float Score { get; set; }
    }
}
