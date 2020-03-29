using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class TechnologyDetailMap : EntityTypeConfiguration<TechnologyDetail>
    {
        public TechnologyDetailMap()
        {
            this.HasKey(c => c.Id);

            this.HasRequired(c => c.Technology)
                .WithMany(c => c.TechnologyDetails)
                .HasForeignKey(c => c.TechnologyId)
                .WillCascadeOnDelete();

            //this.HasOptional(u => u.SavedInRequest)
            //     .WithMany(u => u.TechnologySaveIn)
            //     .HasForeignKey(u => u.SavedInRequestId)
            //     .WillCascadeOnDelete(false);
        }
    }
}
