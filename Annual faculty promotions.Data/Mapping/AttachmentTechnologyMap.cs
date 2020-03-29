using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    class AttachmentTechnologyMap:EntityTypeConfiguration<AttachmentTechnology>
    {
        public AttachmentTechnologyMap()
        {
            this.HasKey(c => c.Id);

            this.HasRequired(c=>c.Technology)
                .WithMany(c=>c.AttachmentTechnologies)
                .HasForeignKey(c=>c.TechnologyId)
                .WillCascadeOnDelete();
        }
    }
}
