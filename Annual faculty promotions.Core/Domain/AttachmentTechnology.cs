using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;

namespace Annual_faculty_promotions.Core.Domain
{
    public class AttachmentTechnology : AuditableEntity<long>
    {
        public long TechnologyId { get; set; }
        public virtual Technology Technology { get; set; }

        public string FileName { get; set; }
    }
}
