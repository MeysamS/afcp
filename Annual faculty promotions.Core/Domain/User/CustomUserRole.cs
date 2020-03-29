using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Annual_faculty_promotions.Core.Domain.User
{
    public class CustomUserRole : IdentityUserRole<int>
    {
        public virtual UnivercityStructure Department { get; set; }
    }
}
