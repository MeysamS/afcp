using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;

namespace Annual_faculty_promotions.Core.Domain
{
    public class TechnologyDetail : AuditableEntity<long>
    {
        //[Index("IX_TechnologyDetail", IsUnique = true, Order = 1)]
        public long TechnologyId { get; set; }
        public virtual Technology Technology { get; set; }
        
       // [Index("IX_TechnologyDetail", IsUnique = true, Order = 2)]
        public long RequestId { get; set; }
        public float Score { get; set; }
    }
}
