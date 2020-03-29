using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    // فعالیت های آموزشی یا پژوهشی موظف
  public  class EducationalResearch:AuditableEntity<long>
  {
      // آموزشی یا پژوهشی
      public int EducationalResearchStatus { get; set; }

      // تعیین نیمسال
      public int Term { get; set; }

      public string Subject { get; set; }

      //شماره درس
      public string CourseNo { get; set; }

       // سمت در طرح
      public int? ResearchPost { get; set; }

      // تعداد واحد
      public int? UnitCount { get; set; }

      // تاریخ شروع طرح
      public string BeginDate { get; set; }

      //مفطع تحصیلی
      public int? GradeEducation { get; set; }

      // تاریخ پایان طرح
      public string EndDate { get; set; }

      // تعداد دانشجو
      public int? StudentCount { get; set; }

      // تعداد فرم ارزشیلبی
      public int? EvaluationFormCount { get; set; }

      //کیقیت تدریس
      public float QualityTeaching { get; set; }

      // واحد معادل
      public float UnitEqual { get; set; }

      // ارسال به موقع یا تاخیر نمرات
      public bool? SendOnTimeOrScoresDelay { get; set; } 

    // شماره درخواست
      public long RequestId { get; set; }
      public virtual Request Request { get; set; }

      // پیوست ها
      public virtual ICollection<AttachmentResearch> AttachmentResearches { get; set; }

  

  }
}
