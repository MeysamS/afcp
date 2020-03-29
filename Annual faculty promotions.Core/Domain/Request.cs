using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    // درخواست
    public class Request : AuditableEntity<long>
    {
        //[Index("IX_Request", IsUnique = true, Order = 1)]
        public int UserId { get; set; }

        public virtual AppUser User { get; set; }

        //[Index("IX_Request", IsUnique = true, Order = 2)]
        public int Term { get; set; }

        // وضعیت استخدام
        public EmploymentStatus EmploymentStatus { get; set; }

         
        [NotMapped]
        public string EmploymentStatusName => EmploymentStatus.ToString().Replace('_',' ');

        // مرتبه علمی
        public AcademicDegree AcademicDegree { get; set; }

        // ساختار
        public int UnivercityStructureId { get; set; }
        public UnivercityStructure UnivercityStructure { get; set; }

        // پایه
        public int Grade { get; set; }

        // تاریخ آخرین ترفیع
        [UIHint("PersianDatePicker")]
        public DateTime? LastDateGrade { get; set; }

        // وضعیت درخواست
        public RequestStatus Status { get; set; }

        //   حضور در دانشگاه هفته ای - ساعتی 
        public byte? PresenceInUnivercity { get; set; }

        public virtual Archive Archive { get; set; }

        // فعالیت های آموزشی یا پژوهشی موظف
        public virtual ICollection<EducationalResearch> EducationalResearches { get; set; }

        // پایان نامه ها 
        public virtual ICollection<Dissertation> Dissertations { get; set; }

        // پژوهشی فناوری ها 
        public virtual ICollection<Technology> Technologies { get; set; }
        // علمی - اجرایی
        public virtual ICollection<ScientificExecutive> ScientificExecutives { get; set; }
        public virtual ICollection<FurtherInformation> FurtherInformations { get; set; } 
        public virtual ICollection<Cartable> Cartables { get; set; }
        public virtual ICollection<Technology> TechnologySaveIn { get; set; }
        
    }
}
