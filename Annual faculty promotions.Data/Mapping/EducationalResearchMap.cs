using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
   public class EducationalResearchMap:EntityTypeConfiguration<EducationalResearch>
   {
       public EducationalResearchMap()
       {
           this.HasKey(c => c.Id);

           this.HasRequired(c=>c.Request)
               .WithMany(c=>c.EducationalResearches)
               .HasForeignKey(c=>c.RequestId)
               .WillCascadeOnDelete(false);
       }
    }
}
