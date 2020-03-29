using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    // اطلاعات تکمیلی
    public class FurtherInformation:AuditableEntity<long>
    {
        // شماره درخواست
        public long RequestId { get; set; }
        public virtual Request Request { get; set; }

        // پست اجرایی
        public bool HasExecutivePosts { get; set; }

        // عنوان پست(سمت)
        public string ExecutivePostName { get; set; }
        /// <summary>
        ///  مامور به تحصیل در مقطع دکتری
        /// پیوسات دارد
        /// </summary>

        public bool OfficersStudyPhD { get; set; }

        // نوع همکاری نیمه وقت بدون هیچگونه ارتباط با سایر دانشگاهها
        public bool TypeOfPartTimeWorkNoconnectionOtherUniversity { get; set; }
        /// <summary>
        ///  نوع همکاری نیمه وقت اما به صورت تمام وقت شاغل در سایر دانشگاهها
        /// پیوست دارد : ارائه حکم کارگزینی مبنی بر اعطای پایه سالیانه در دانشگاه دولتی
        /// </summary>

        public bool TypeOfPartTimeButEmployeesOtherUniversity { get; set; }

        /// <summary>
        /// نوع همکاری تمام وقت اما بازنشسته از سایر دانشگاهها
        /// پیوست دارد
        /// </summary>
        public bool TypeOfFullTimeButRetired { get; set; }


        /// <summary>
        /// نوع همکاری تمام وقت اما غیر بازنشسته در صورت استفاء
        /// پیوست دارد
        /// </summary>
        public bool TypeOfFullTimeButNoRetired { get; set; }

        /// <summary>
        /// اخذ مدرک دانشوری با استفاده از آیین نامه طرح دانشوری دانشگاه
        /// </summary>
        public bool GraduationDaneshvari { get; set; }

        // جانباز
        public bool IsVeteran { get; set; }
        public VeteranType? VeteranType { get; set; }

        // آزادگان،اسرا،مفقودین
        public bool FreedmanOrCaptive { get; set; }
        public FreedmanOrCaptiveType? FreedmanOrCaptiveType { get; set; }

        // رزمنده
        public bool Fighter { get; set; }
        public FighterType? FighterType { get; set; }

        // دارای دکترای تخصصی غیر مامور به تحصیل
        public bool HasPhDNoOfficersStudy { get; set; }

        // دارای مرخصی استعلاجی زایمان
        public bool HasMaternityLeave { get; set; }

        /// <summary>
        /// استفاده از طرح مشمولین متخصص دوره دکتری تخصصی
        /// </summary>
        public bool HasPhDIncludingMilitary { get; set; }

        /// <summary>
        ///  گذراندن دوره پسا دکتری
        /// پیوست دارد - گواهی دوره
        /// </summary>

        public bool PassPhDPassa { get; set; }

        /// <summary>
        /// درحال گذراندن فرصت مطالعاتی
        /// پیوست دارد - مجوز سازمان مرکزی
        /// </summary>
        public bool TakingSabbatical { get; set; }
        
        /// <summary>
        /// دارای  مطالبه پایه معوق
        /// </summary>
        public bool HasBasicDelayedPreviousYears { get; set; }

             // پیوست های مربوط به مستندات مطالبه پایه معوق
        public virtual ICollection<AttachmentBasicDelayedPreviousYears> AttachmentBasicDelayedPreviousYearses { get;
            set; }   
        
        // پیوست های مربوطه
        public virtual ICollection<AttachmentFurtherInformation> AttachmentFurtherInformations { get; set; }

    }
}
