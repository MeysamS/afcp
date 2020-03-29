using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;



namespace Annual_faculty_promotions.Data.Mapping
{
   public class AppUserMap:EntityTypeConfiguration<AppUser>
    {
        public AppUserMap()
        {
            this.HasOptional(a=>a.Profile)
                .WithRequired(a=>a.AppUser)                   
                .WillCascadeOnDelete();

            //this.HasRequired(a => a.BaseUser)
            //    .WithOptional(a => a.User)
            //    .WillCascadeOnDelete(false);
        }
    }
}
