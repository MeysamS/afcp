using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Log : AuditableEntity<long>
    {
        public int UserId { get; set; }
        public virtual AppUser User { get; set; }
        public Operations Operation { get; set; }
        public OperationsDetail OperationDetail { get; set; }
        public string Description { get; set; }
        public long? MessagingId { get; set; }
        public virtual Messaging Messaging { get; set; }
        
    }
}
