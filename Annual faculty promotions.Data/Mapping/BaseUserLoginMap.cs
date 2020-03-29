using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain.User;

namespace Annual_faculty_promotions.Data.Mapping
{
   public class BaseUserLoginMap:EntityTypeConfiguration<BaseUserLogin>
   {
       public BaseUserLoginMap()
       {
           this.HasKey(c =>c.BaseUserLoginId);

           //this.HasOptional(a => a.User)               
           //              .WithRequired(a => a.BaseUserLogin)
           //              .Map(x=>x.MapKey("BaseUserLoginId"))
           //              .WillCascadeOnDelete(false);
       }
    }
}
