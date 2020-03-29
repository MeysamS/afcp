using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    //پژوهشی فناوری
    public class Technology:AuditableEntity<long>
    {
        // شماره درخواست
        public long RequestId { get; set; }
        public virtual Request Request { get; set; }
        
        // نوع پژوهشی فناوری
        public TechnologyType TechnologyType { get; set; }

        public string Subject { get; set; }

        // تاریخ انتشار یا ارائه
        public string PresentationDate { get; set; }

        // محل ارائه
        public string PlacePresentation { get; set; }

        // اسامی همکاران
        public string PartnersNames { get; set; }

        // امتیاز
        public float Score { get; set; }
        public float RemindScore { get; set; }
        
        // پیوست های مربوطه
        public virtual ICollection<AttachmentTechnology> AttachmentTechnologies { get; set; }
        public virtual ICollection<TechnologyDetail> TechnologyDetails { get; set; }
    }
}
