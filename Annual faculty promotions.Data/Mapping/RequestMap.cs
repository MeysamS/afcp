using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Annual_faculty_promotions.Core.Domain;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class RequestMap : EntityTypeConfiguration<Request>
    {
        public RequestMap()
        {
            this.HasKey(c => c.Id);


            this.HasRequired(c => c.User)
                .WithMany(c => c.Requests)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete();



            this.HasRequired(c => c.UnivercityStructure)
                        .WithMany(c => c.Requests)
                        .HasForeignKey(c => c.UnivercityStructureId)
                        .WillCascadeOnDelete(false);

            this.HasOptional(c => c.Archive)
                .WithRequired(c => c.Request)
                .WillCascadeOnDelete();


        }
    }
}
