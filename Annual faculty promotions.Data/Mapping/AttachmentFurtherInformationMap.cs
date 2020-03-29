using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
   public class AttachmentFurtherInformationMap:EntityTypeConfiguration<AttachmentFurtherInformation>
   {
       public AttachmentFurtherInformationMap()
       {
           this.HasKey(c => c.Id);

           this.HasRequired(c=>c.FurtherInformation)
               .WithMany(c=>c.AttachmentFurtherInformations)
               .HasForeignKey(c=>c.FurtherInformationId)
               .WillCascadeOnDelete();
       }
    }
}
