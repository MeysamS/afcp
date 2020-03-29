using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Common;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Core.Domain
{
    public class Access : Entity<long>
    {
        public int UserId { get; set; }
        public virtual AppUser User { get; set; }


        public long SourceCode { get; set; }
        public string SourceName { get; set; }

    }
}
