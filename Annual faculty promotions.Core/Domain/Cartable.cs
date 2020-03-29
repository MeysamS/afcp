using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;
using Annual_faculty_promotions.Core.Enums;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Cartable : AuditableEntity<long>
    {
        public long RequestId { get; set; }
        public virtual Request Request { get; set; }

        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }

        public int UserSenderId { get; set; }
        public virtual AppUser UserSender { get; set; }

        public int UserReciveId { get; set; }
        public virtual AppUser UserRecive { get; set; }

        public bool Active { get; set; }

        public string Description { get; set; }


        // وضعیت - عادی یا برگشت
        public CurrentCartable CurrentCartable { get; set; }

    }
}
