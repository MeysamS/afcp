using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    public class AttachmentFurtherInformation : AuditableEntity<long>
    {
        // شماره اطلاعات تکمیلی
        public long FurtherInformationId { get; set; }
        public virtual FurtherInformation FurtherInformation { get; set; }
        
        // نوع پیوست اطلاعات تکمیلی
        public FurtherInformationType FurtherInformationType { get; set; }

        //[RegularExpression(@"^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.pdf|.PDF)$", ErrorMessage = "فرمت فایل انتخابی باید از نوع تصویر یا پی دی اف باشد!")]
        public string ImageName { get; set; }
    }
}
