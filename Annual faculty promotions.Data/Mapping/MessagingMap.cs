using Annual_faculty_promotions.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Annual_faculty_promotions.Data.Mapping
{
    public class MessagingMap : EntityTypeConfiguration<Messaging>
    {
        public MessagingMap()
        {
            this.HasKey(c => c.Id);


            this.HasRequired(c => c.UserSender)
                .WithMany(c => c.UserSenderMessagings)
                .HasForeignKey(c => c.UserSenderId)
                .WillCascadeOnDelete(false);

            this.HasRequired(c => c.UserReciever)
                .WithMany(c => c.UserRecieverMessagings)
                .HasForeignKey(c => c.UserRecieverId)
                .WillCascadeOnDelete(false);
        }
    }
}
