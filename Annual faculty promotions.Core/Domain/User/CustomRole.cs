using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Annual_faculty_promotions.Core.Domain.User
{
    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole()
        {
        }

        public CustomRole(string name,string persianName)
        {
            Name = name;
            PersianName = persianName;
        }

        public virtual Stage Stage { get; set; }

        public string PersianName { get; set; }


    }
}
