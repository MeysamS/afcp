using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Archive : AuditableEntity<long>
    {
        public virtual Request Request { get; set; }
        public int UserId { get; set; }
        public virtual AppUser User { get; set; }

        // رئیس دانشکده / گروه آموزشی
        public int? ChiefCommiteId { get; set; }
        public virtual AppUser ChiefCommite { get; set; }

        // رئیس دانشکده / گروه آموزشی
        public int? ChiefId { get; set; }
        public virtual AppUser Chief { get; set; }

        // موافقت ، عدم موافقت
        public bool IsOpinionCommite { get; set; }

        public string Description { get; set; }
       // public string DescScore { get; set; }

        // امتیاز آموزشی
        public float EducationScore { get; set; }

        // امتیاز پژوهشی
        public float ResearchScore { get; set; }

        // امتیاز راهنمایی پروژه
        public float DissertationScore { get; set; }

        // امتیاز پژوهشی - فن آوری
        public float TechnologyScore { get; set; }

        // امتیاز اجرایی
        public float ExecutiveScore { get; set; }

        // مجموع امتیاز
        public float SumScore { get; set; }

        // پایه
        public int Grade { get; set; }
    }
}

