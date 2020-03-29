using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Data.Mapping
{
    class CustomUserRoleMap:EntityTypeConfiguration<CustomUserRole>
    {
        public CustomUserRoleMap()
        {
            this.HasKey(k => new {k.UserId, k.RoleId});
        }
    }
}
