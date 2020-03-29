using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class AttachmentResearchMap : EntityTypeConfiguration<AttachmentResearch>
    {
        public AttachmentResearchMap()
        {
            this.HasKey(c => c.Id);

            this.HasRequired(c=>c.EducationalResearch)
                .WithMany(c=>c.AttachmentResearches)
                .HasForeignKey(c=>c.EducationaResearchId)
                .WillCascadeOnDelete();
        }
    }
}
