using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    public class AttachmentResearch : AuditableEntity<long>
    {


        public long EducationaResearchId { get; set; }
        public virtual EducationalResearch EducationalResearch { get; set; }


        public string FileName { get; set; }

      

    }
}
