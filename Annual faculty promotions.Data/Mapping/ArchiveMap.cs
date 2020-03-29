using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class ArchiveMap : EntityTypeConfiguration<Archive>
    {
        public ArchiveMap()
        {
            this.HasKey(x => x.Id);

            this.HasRequired(x => x.User)
                .WithMany(x => x.Archives)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);


            this.HasOptional(c => c.Chief)
                .WithMany(c => c.ChiefRequests)
                .HasForeignKey(c => c.ChiefId)
                .WillCascadeOnDelete(false);


            this.HasOptional(c => c.ChiefCommite)
              .WithMany(c => c.ChiefCommiteRequests)
              .HasForeignKey(c => c.ChiefCommiteId)
              .WillCascadeOnDelete(false);
        }
    }
}
