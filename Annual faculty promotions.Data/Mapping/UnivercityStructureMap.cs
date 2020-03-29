using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class UnivercityStructureMap : EntityTypeConfiguration<UnivercityStructure>
    {
        public UnivercityStructureMap()
        {
            this.HasOptional(c => c.Parent)
             .WithMany()
             .HasForeignKey(c => c.ParentId)
             .WillCascadeOnDelete(false);
        }
    }
}
