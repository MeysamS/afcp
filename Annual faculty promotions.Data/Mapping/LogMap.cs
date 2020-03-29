using Annual_faculty_promotions.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class LogMap : EntityTypeConfiguration<Log>
    {
        public LogMap()
        {
            this.HasKey(c => c.Id);



            this.HasOptional(a => a.Messaging)
                .WithMany(a => a.Logs)
                .HasForeignKey(a=>a.MessagingId)
                .WillCascadeOnDelete(false);

            this.HasRequired(c => c.User)
                .WithMany(c => c.Logs)
                .HasForeignKey(c => c.UserId)
                .WillCascadeOnDelete(false);
        }
    }
}
